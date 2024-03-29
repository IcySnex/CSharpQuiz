﻿using CSharpQuiz.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace CSharpQuiz.Services;

public class DynamicRuntime
{
    readonly ILogger<DynamicRuntime> logger;

    readonly List<MetadataReference> references = [];

    public DynamicRuntime(
        ILogger<DynamicRuntime> logger)
    {
        this.logger = logger;

        logger.LogInformation("Ersetelle Referenzen für dynamische Assembly.");
        unsafe
        {
            typeof(object).Assembly.TryGetRawMetadata(out byte* systemBlob, out int systemLength);
            AssemblyMetadata systemMetadata = AssemblyMetadata.Create(ModuleMetadata.CreateFromMetadata((IntPtr)systemBlob, systemLength));

            typeof(Shared.Buch).Assembly.TryGetRawMetadata(out byte* sharedBlob, out int sharedLenght);
            AssemblyMetadata sharedMetadata = AssemblyMetadata.Create(ModuleMetadata.CreateFromMetadata((IntPtr)sharedBlob, sharedLenght));

            Assembly.Load("System.Runtime").TryGetRawMetadata(out byte* runtimeBlob, out int runtimeLength);
            AssemblyMetadata runtimeMetadata = AssemblyMetadata.Create(ModuleMetadata.CreateFromMetadata((IntPtr)runtimeBlob, runtimeLength));

            Assembly.Load("System.Linq").TryGetRawMetadata(out byte* linqBlob, out int linqLength);
            AssemblyMetadata linqMetadata = AssemblyMetadata.Create(ModuleMetadata.CreateFromMetadata((IntPtr)linqBlob, linqLength));

            references.Add(systemMetadata.GetReference());
            references.Add(sharedMetadata.GetReference());
            references.Add(runtimeMetadata.GetReference());
            references.Add(linqMetadata.GetReference());
        }
    }


    public byte[] Compile(
        string sourceCode)
    {
        logger.LogInformation("Prepariere Source Code für Kompilieren.");
        SourceText sourceText = SourceText.From(sourceCode);
        CSharpParseOptions options = CSharpParseOptions.Default
            .WithLanguageVersion(LanguageVersion.CSharp12);

        SyntaxTree parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceText, options);

        logger.LogInformation("Kompilieren gestartet...");
        CSharpCompilation compilation = CSharpCompilation.Create("DynamicRuntimeAssembly.dll",
            [parsedSyntaxTree],
            references,
            new(outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

        using MemoryStream assemblyStream = new();
        EmitResult result = compilation.Emit(assemblyStream);

        if (!result.Success)
        {
            IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

            logger.LogError("Kompilieren fehlgeschlagen: {failures}.", failures);
            throw new CompilationFailedException("Compilation of source code failed.", failures);
        }

        logger.LogInformation("Kompilieren vollendet...");
        assemblyStream.Seek(0, SeekOrigin.Begin);
        return assemblyStream.ToArray();
    }

    public void Execute(
        byte[] compiledAssembly,
        string method,
        object?[]? args = null)
    {
        logger.LogInformation("Dynamische Assembly Laden gestartet...");
        using MemoryStream asm = new(compiledAssembly);
        UnloadableAssemblyLoadContext assemblyLoadContext = new();

        try
        {
            Assembly assembly = assemblyLoadContext.LoadFromStream(asm);
            System.Reflection.TypeInfo? type = assembly.DefinedTypes.FirstOrDefault();

            logger.LogInformation("Mehtode ausführen: {method}.", method);
            MethodInfo? methodInfo = type?.DeclaredMethods.FirstOrDefault(methodInfo => methodInfo.Name == method);
            if (methodInfo is null)
            {
                logger.LogError("Konnte Methode nicht finden: {method}.", method);
                throw new Exception($"Method with given name could not be found: {method}.");
            }

            methodInfo.Invoke(null, args);
        }
        finally
        {
            UnloadAssembly(assemblyLoadContext);
        }
    }

    public T? Execute<T>(
        byte[] compiledAssembly,
        string method,
        object?[]? args = null)
    {
        logger.LogInformation("Dynamische Assembly Laden gestartet...");
        using MemoryStream asm = new(compiledAssembly);
        UnloadableAssemblyLoadContext assemblyLoadContext = new();

        try
        {
            Assembly assembly = assemblyLoadContext.LoadFromStream(asm);
            System.Reflection.TypeInfo? type = assembly.DefinedTypes.FirstOrDefault();

            logger.LogInformation("Mehtode ausführen: {method}.", method);
            MethodInfo? methodInfo = type?.DeclaredMethods.FirstOrDefault(methodInfo => methodInfo.Name == method);
            if (methodInfo is null)
            {
                logger.LogError("Konnte Methode nicht finden: {method}.", method);
                throw new Exception($"Method with given name could not be found: {method}.");
            }

            object? result = methodInfo.Invoke(null, args);
            return result is null ? default : (T)result;
        }
        finally
        {
            UnloadAssembly(assemblyLoadContext);
        }
    }


    void UnloadAssembly(
        UnloadableAssemblyLoadContext assemblyLoadContext)
    {
        logger.LogInformation("Dynamische Assembly Entladen gestartet...");
        assemblyLoadContext.Unload();
        WeakReference assemblyLoadContextWeakRef = new(assemblyLoadContext);

        for (int i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++)
            GC.Collect();
    }
}


public class CompilationFailedException : Exception
{
    public IEnumerable<Diagnostic> Failures { get; }


    public CompilationFailedException(
        IEnumerable<Diagnostic> failures)
    {
        Failures = failures;
    }

    public CompilationFailedException(
        string? message,
        IEnumerable<Diagnostic> failures) : base(message)
    {
        Failures = failures;
    }

    public CompilationFailedException(
        string? message,
        Exception? innerException,
        IEnumerable<Diagnostic> failures) : base(message, innerException)
    {
        Failures = failures;
    }
}