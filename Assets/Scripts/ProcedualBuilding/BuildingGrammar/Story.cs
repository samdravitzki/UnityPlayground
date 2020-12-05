
namespace Grammar.Building
{
    public class Story
    {
        int level; // Track the level the story is on (rule of thumb for extra data = if best calculated during the generation of the grammar)
        Wall[] walls;

        public int Level { get => level; }
        public Wall[] Walls { get => walls; }

        public Story(int level, Wall[] walls)
        {
            this.level = level;
            this.walls = walls;
        }

        public override string ToString()
        {
            string story = $"Story:({level}; {walls.Length}): ";
            foreach (Wall wall in walls)
            {
                story += $"{wall}, ";
            }

            return story;
        }
    }
}
