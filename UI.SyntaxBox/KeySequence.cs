using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace UI.SyntaxBox;

/// <summary>
/// Entry point to create a multi-key binding sequence. Simply 
/// introduces a new type converter for the Gesture property, to  parse
/// sequences on the form Modifier+Key1, Key2, ..., KeyN.
/// Usage:
/// <c>
/// <Window.InputBindings>
///     <syntax:KeySequenceBinding Gesture="Ctrl+A, B" Command="..." />
///     <syntax:KeySequenceBinding Gesture="Ctrl+A, C" Command="..." />1
/// </Window.InputBindings>
/// </c>
/// </summary>
public class KeySequenceBinding : InputBinding
{
    [TypeConverter(typeof(KeySequenceConverter))]
    public override InputGesture Gesture
    {
        get => base.Gesture;
        set => base.Gesture = value;
    }
}

/// <summary>
/// Gesture implementation. Keeps a sequence of keys and advances the 
/// pointer in the sequence if a keystroke matches.
/// </summary>
public class KeySequenceGesture : KeyGesture
{
    ModifierKeys modifiers;
    IList<Key> keys;
    public int pointer = 0;


    /// <summary>
    /// Ensures that the default constructor cannot be used externally.
    /// </summary>
    /// <param name="DisplayString"></param>
    private KeySequenceGesture(string DisplayString) : base(Key.None, ModifierKeys.None, DisplayString) { }

    /// <summary>
    /// Initializing constructor.
    /// </summary>
    /// <param name="Modifiers">Modifier keys</param>
    /// <param name="Keys">List of keys in the sequence</param>
    /// <param name="DisplayString">Display string for the sequence</param>
    public KeySequenceGesture(
        ModifierKeys Modifiers,
        IList<Key> Keys,
        string DisplayString)
        : this(DisplayString)
    {
        modifiers = Modifiers;
        keys = Keys;
    }


    /// <summary>
    /// Matches an input event to the current state of the instance.
    /// If a match is found, the pointer is advanced forward in the sequence.
    /// Returns true only if the match is made on the LAST element in the 
    /// sequence.
    /// </summary>
    /// <param name="targetElement"></param>
    /// <param name="inputEventArgs"></param>
    /// <returns></returns>
    public override bool Matches(
        object targetElement,
        InputEventArgs inputEventArgs)
    {
        KeyEventArgs keyArgs = (inputEventArgs as KeyEventArgs);
        if (keyArgs == null)
            return (false);

        if (keyArgs.IsRepeat)
        {
            return (false);
        }
        // Wrong input => fail and reset.
        if (Keyboard.Modifiers != modifiers || keyArgs.Key != keys[pointer])
        {
            pointer = 0;
            return (false);
        }
        // Matches current element in sequence => set to handled and advance
        else
        {
            keyArgs.Handled = true;
            pointer++;
        }

        // If we now passed the tail of the sequence, return true
        if (pointer >= keys.Count)
        {
            pointer = 0;
            return (true);
        }
        return (false);
    }
}

/// <summary>
/// Converts a string into either a KeyGEsture or a KeySequenceGesture 
/// if multiple keys are specified.
/// </summary>
public class KeySequenceConverter : TypeConverter
{
    /// <summary>
    /// Determines whether this instance can convert from the specified sourceType.
    /// Can convert from string and nothing else.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="sourceType">Type of the source.</param>
    /// <returns>
    ///   <c>true</c> if this instance [can convert from] the specified context; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
        sourceType == typeof(string);


    /// <summary>
    /// Converts from.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">value</exception>
    /// <exception cref="ArgumentException">Argument must be of string type - value</exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string sInput = value as string ?? throw new ArgumentException("Argument must be of string type", nameof(value));
        ModifierKeys modifiers = ModifierKeys.None;

        int lastSplit = sInput.LastIndexOf('+');
        if (lastSplit > 1)
        {
            string sMod = sInput.Substring(0, lastSplit + 1);
            modifiers = (ModifierKeys)new ModifierKeysConverter().ConvertFromString(context, culture, sMod);
        }

        string sKeys = sInput.Substring(lastSplit + 1, sInput.Length - lastSplit - 1);

        KeyConverter keyConv = new KeyConverter();
        var keys = (sKeys)
            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select((x) => (Key)keyConv.ConvertFromString(context, culture, x.Trim()))
            .ToList();

        return (keys.Count == 1)
            ? new KeyGesture(keys[0], modifiers, sInput)
            : new KeySequenceGesture(modifiers, keys, sInput);
    }
}
