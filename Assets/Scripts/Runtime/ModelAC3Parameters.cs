public readonly struct ModelAC3Parameters
{
    public readonly int width;
    public readonly int height;
    public readonly bool periodic;
    public readonly WFCTile[] tiles;

    public ModelAC3Parameters(int width, int height, bool periodic, WFCTile[] tiles)
    {
        this.width = width;
        this.height = height;
        this.periodic = periodic;
        this.tiles = tiles;
    }
};
