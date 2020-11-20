using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestFireModel
{
    public class ForestModel
    {
        public enum Landscape : byte
        {
            Empty = 0,
            Tree = 1,
            Fire = 2
        }

        private Landscape[,] _matrix;

        public Landscape[,] GetState()
        {
            return _matrix;
        }

        private static readonly Random rand = new Random();

        private float _forestFireChance { get; set; }

        private float _treeGrowChance { get; set; }

        public int HeightMap { get; set; }

        public int WidthMap { get; set; }

        protected ForestModel(float f, float p, int height, int width)
        {
            _forestFireChance = f;
            _treeGrowChance = p;
            HeightMap = height;
            WidthMap = width;

            Initialize();
        }

        public static Result<ForestModel> Create(float f, float p, int height, int width)
        {
            var errors = new List<string>();

            if (!(0 <= f && f <= 1)) errors.Add("Вероятность возгорания дерева только [0,1]");
            if (!(0 <= p && p <= 1)) errors.Add("Вероятность заполнением дерева только [0,1]");
            if (height < 1) errors.Add("Высота не может быть меньше единицы");
            if (width < 1) errors.Add("Ширина не может быть меньше единицы");

            if (errors.Any())
            {
                return Result<ForestModel>.Fail(errors);
            }

            return Result<ForestModel>.Success(new ForestModel(f, p, height, width));
        }

        private void Initialize()
        {
            _matrix = new Landscape[HeightMap, WidthMap];
            _matrix.Initialize();
        }

        public Landscape[,] Step()
        {
            var newForest = (Landscape[,])_matrix.Clone();

            int numRows = _matrix.GetLength(0);
            int numCols = _matrix.GetLength(1);

            for (int x = 1; x < numRows - 1; x++)
            {
                for (int y = 1; y < numCols - 1; y++)
                {
                    switch (_matrix[x, y])
                    {
                        case Landscape.Empty:
                            if (rand.NextDouble() < _treeGrowChance)
                                newForest[x, y] = Landscape.Tree;
                            break;

                        case Landscape.Tree:
                            if (NeighborLandscape(_matrix, x, y, Landscape.Fire) || rand.NextDouble() < _forestFireChance)
                                newForest[x, y] = Landscape.Fire;
                            break;

                        case Landscape.Fire:
                            newForest[x, y] = Landscape.Empty;
                            break;
                    }
                }
            }

            _matrix = newForest;
            return newForest;
        }

        private bool NeighborLandscape(Landscape[,] state, int x, int y, Landscape value)
        {
            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    if (r == 0 && c == 0)
                        continue;

                    if (state[x + r, y + c] == value)
                        return true;
                }
            }

            return false;
        }
    }
}
