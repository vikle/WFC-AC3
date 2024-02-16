using System.Collections.Generic;
using Random = System.Random;

public sealed class ModelAC3
{
    Random m_random;
    readonly int m_width;
    readonly int m_height;
    readonly bool m_periodic;
    readonly List<WFCTile>[,] m_wave;
    readonly int[,] m_possibilities;

    readonly (int, int)[] m_queue;
    int m_queueSize;
    int m_queueHead;
    int m_queueTail;

    readonly List<double> m_distribution;
    
    static readonly int[] sr_dirX = { 0, 1, 0, -1 };
    static readonly int[] sr_dirY = { 1, 0, -1, 0 };

    
    public ModelAC3(in ModelAC3Parameters p)
    {
        int width = p.width; 
        int height = p.height;
        
        m_width = width;
        m_height = height;
        m_periodic = p.periodic;
        m_possibilities = new int[width, height];
        m_wave = new List<WFCTile>[width, height];
        m_queue = new (int, int)[width * height];

        var tiles = p.tiles;
        int pattern_count = tiles.Length;
        m_distribution = new List<double>(pattern_count);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                m_wave[x, y] = new List<WFCTile>(tiles);
                m_possibilities[x, y] = pattern_count;
            }
        }
    }
    
    
    public void Run(int seed, int limit = -1)
    {
        m_random = new Random(seed);

        for (int l = 0; l < limit || limit < 0; l++)
        {
            if (NextUnobserved(out int x, out int y) == false) break;
            Observe(x, y);
            Propagate();
        }
    }
    

    private bool NextUnobserved(out int minX, out int minY)
    {
        minX = -1;
        minY = -1;
        int min_entropy = int.MaxValue;

        for (int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                int remaining = m_possibilities[x, y];
                if (remaining <= 1 || remaining >= min_entropy) continue;
                min_entropy = remaining;
                minX = x; 
                minY = y;
            }
        }

        return (minX > -1 && minY > -1);
    }
    
    private void Observe(int x, int y)
    {
        int remaining = m_possibilities[x, y];
        var domain = m_wave[x, y];
        
        m_distribution.Clear();

        for (int i = 0; i < remaining; i++)
        {
            m_distribution.Add(domain[i].weight);
        }

        int pattern = m_random.NextArrayElementIndex(m_distribution);
        var collapsed_tile = domain[pattern];
        
        domain.Clear();
        domain.Add(collapsed_tile);
        m_possibilities[x, y] = 1;

        Enqueue(x, y);
    }

    private void Propagate()
    {
        while (m_queueSize > 0)
        {
            (int x, int y) = m_queue[m_queueHead];
            MoveQueueToNext(ref m_queueHead);
            m_queueSize--;

            var tile = m_wave[x, y][0];

            for (int direction = 0; direction < 4; direction++)
            {
                int nx = (x + sr_dirX[direction]);
                int ny = (y + sr_dirY[direction]);

                if (m_periodic)
                {
                    if (nx < 0) nx += m_width;
                    else if (nx >= m_width) nx -= m_width;
                    if (ny < 0) ny += m_height;
                    else if (ny >= m_height) ny -= m_height;
                }
                else
                {
                    if (nx < 0) continue;
                    if (ny < 0) continue;
                    if (nx >= m_width) continue;
                    if (ny >= m_height) continue;
                }

                int remaining = m_possibilities[nx, ny];
                if (remaining <= 1) continue;
        
                var neighbor_domain = m_wave[nx, ny];
                var wvc_dir = (EDirection)direction;
                
                foreach (var neighbor in neighbor_domain.ToArray())
                {
                    if (tile.CanAppendTile(neighbor, wvc_dir)) continue;
                    if (neighbor_domain.Remove(neighbor) == false) continue;
                    remaining--;
                }

                m_possibilities[nx, ny] = remaining;
        
                if (remaining <= 1) Enqueue(nx, ny);
            }
        }
    }

    private void Enqueue(int x, int y)
    {
        m_queueSize++;
        m_queue[m_queueTail] = (x, y);
        MoveQueueToNext(ref m_queueTail);
    }

    private void MoveQueueToNext(ref int index)
    {
        if (++index == m_queue.Length) index = 0;
    }

    public WFCTile[,] GetSolvedGrid()
    {
        var grid = new WFCTile[m_width, m_height];

        for (int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                grid[x, y] = m_wave[x, y][0];
            }
        }

        return grid;
    }
};
