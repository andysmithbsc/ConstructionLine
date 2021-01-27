using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        private SearchResults _searchResults;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;

            InitialiseSearchResults();
        }
        
        public SearchResults Search(SearchOptions options)
        {
            var matches = Matches(_shirts, options.Colors, options.Sizes);

            var colorGroups = matches
                .GroupBy(x => x.Color)
                .Select(group => new ColorCount(group.Key, group.Count()));

            foreach (var group in colorGroups)
            {
                var colorCount = _searchResults.ColorCounts.First(x => x.Color.Name == group.Color.Name);
                colorCount.Count = group.Count;
            }

            var sizeGroups = matches
                .GroupBy(x => x.Size)
                .Select(group => new SizeCount(group.Key, group.Count()))
                .ToList();

            foreach (var group in sizeGroups)
            {
                var sizeCount = _searchResults.SizeCounts.First(x => x.Size.Name == group.Size.Name);
                sizeCount.Count = group.Count;
            }

            return _searchResults;
        }

        public IList<Shirt> Matches(IEnumerable<Shirt> allShirts, IEnumerable<Color> colors, IEnumerable<Size> sizes)
        {
            var sizeIds = sizes.Select(x => x.Id);

            var colorIds = colors.Select(x => x.Id);

            var matches = allShirts
                .Where(x => colorIds.Contains(x.Color.Id) || sizeIds.Contains(x.Size.Id))
                .ToList();
            
            return matches;
        }
        
        private void InitialiseSearchResults()
        {
            _searchResults = new SearchResults
            {
                ColorCounts = new List<ColorCount>(),
                SizeCounts = new List<SizeCount>(),
                Shirts = _shirts
            };

            foreach (var color in Color.All)
            {
                _searchResults.ColorCounts.Add(new ColorCount(new Color(color.Id, color.Name), 0));
            }

            foreach (var size in Size.All)
            {
                _searchResults.SizeCounts.Add(new SizeCount(new Size(size.Id, size.Name), 0));
            }
        }
    }
}