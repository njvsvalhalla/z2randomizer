namespace RandomizerCore.Sprites
{
    public interface ISprite
    {
        string Name { get; }
        int[] Sprite { get; }
        int[] OneUp { get; }
        int[] Head { get; }
        int[] Raft { get; }
        int[] Beam { get; }

        //these need better names
        int[] Ow { get; }
        int[] Title { get; }
        int[] Sleeper { get; }
        int[] EndOne { get; }
        int[] EndTwo { get; }
        int[] EndThree { get; }
    }
}