using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.TreeBuilder;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public struct Marker
    {
        public const int InvalidPointer = -1;
        public readonly MarkerType Type;
        public NodeType ElementType;
        public int FirstChild;
        public int LastChild;
        public int LexemeIndex;
        public int NextMarker;
        public int OppositeMarker;
        public int ParentMarker;
        public object UserData;

        public Marker(MarkerType type, int lexemeIndex)
        {
            Type = type;
            LexemeIndex = lexemeIndex;
            OppositeMarker = -1;
            FirstChild = -1;
            LastChild = -1;
            ParentMarker = 0;
            NextMarker = 0;
            ElementType = null;
            UserData = null;
        }

        public static void AddChild(TreeBuilder builder, int node, int child)
        {
            var marker1 = builder.MyProduction[node];
            if (marker1.FirstChild == -1)
            {
                marker1.FirstChild = child;
                marker1.LastChild = child;
            }
            else
            {
                var marker2 = builder.MyProduction[marker1.LastChild];
                marker2.NextMarker = child;
                builder.MyProduction[marker1.LastChild] = marker2;
                marker1.LastChild = child;
            }
            builder.MyProduction[node] = marker1;
        }
    }
}
