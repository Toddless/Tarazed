namespace DataModel
{
    public enum Muscle
    {
        None = 0,
#pragma warning disable SA1300 // Element should begin with upper-case letter.

        // In SVG Picture all groups begin with lower-case.
        // All Elements with lower-case saving ToLower call.
        delts = 1,
        biceps = 2,
        triceps = 4,
        forearms = 8,
        pecs = 16,
        abs = 32,
        traps = 64,
        lats = 128,
        glutes = 256,
        hamstings = 512,
        quads = 1024,
        calves = 2048,
#pragma warning restore SA1300 // Element should begin with upper-case letter
    }
}
