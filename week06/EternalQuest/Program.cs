/*
Week 06 Project: Eternal Quest Program

Extra Features:
 - leveling system
 - badges
 - JSON-like save format
 - user-friendly menu
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EternalQuest
{
    abstract class Goal
    {
        private string _title;
        private string _description;
        private int _points; 

        public string Title { get => _title; set => _title = value; }
        public string Description { get => _description; set => _description = value; }
        public int Points { get => _points; set => _points = value; }

        protected Goal(string title, string description, int points)
        {
            _title = title;
            _description = description;
            _points = points;
        }

        // record event for goal and return points earned
        public abstract int RecordEvent();

        // display status used by the menu
        public abstract string GetStatus();

        // used for save/load
        public abstract string Serialize();

        // factory method to deserialize a goal of the appropriate type
        public static Goal Deserialize(string line)
        {
            var parts = line.Split('|');
            try
            {
                var type = parts[0];
                var title = parts[1];
                var desc = parts[2];
                var points = int.Parse(parts[3]);

                switch (type)
                {
                    case "Simple":
                        bool completed = bool.Parse(parts[4]);
                        return new SimpleGoal(title, desc, points, completed);
                    case "Eternal":
                        int count = int.Parse(parts[4]);
                        return new EternalGoal(title, desc, points, count);
                    case "Checklist":
                        int target = int.Parse(parts[4]);
                        int current = int.Parse(parts[5]);
                        int bonus = int.Parse(parts[6]);
                        return new ChecklistGoal(title, desc, points, target, bonus, current);
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }

    class SimpleGoal : Goal
    {
        private bool _isComplete;

        public SimpleGoal(string title, string description, int points, bool isComplete = false)
            : base(title, description, points)
        {
            _isComplete = isComplete;
        }

        public override int RecordEvent()
        {
            if (_isComplete)
                return 0; 

            _isComplete = true;
            return Points;
        }

        public override string GetStatus()
        {
            return _isComplete ? "[X]" : "[ ]";
        }

        public override string Serialize()
        {
            return $"Simple|{Title}|{Description}|{Points}|{_isComplete}";
        }
    }

    class EternalGoal : Goal
    {
        private int _timesRecorded;

        public EternalGoal(string title, string description, int points, int timesRecorded = 0)
            : base(title, description, points)
        {
            _timesRecorded = timesRecorded;
        }

        public override int RecordEvent()
        {
            _timesRecorded++;
            return Points;
        }

        public override string GetStatus()
        {
            return $"[~] Completed {_timesRecorded} time(s)";
        }

        public override string Serialize()
        {
            return $"Eternal|{Title}|{Description}|{Points}|{_timesRecorded}";
        }
    }

    class ChecklistGoal : Goal
    {
        private int _targetCount;
        private int _currentCount;
        private int _bonusOnCompletion;
        private bool _isComplete;

        public ChecklistGoal(string title, string description, int points, int targetCount, int bonusOnCompletion, int currentCount = 0)
            : base(title, description, points)
        {
            _targetCount = targetCount;
            _currentCount = currentCount;
            _bonusOnCompletion = bonusOnCompletion;
            _isComplete = _currentCount >= _targetCount;
        }

        public override int RecordEvent()
        {
            if (_isComplete) return 0;

            _currentCount++;
            int earned = Points;

            if (_currentCount >= _targetCount)
            {
                _isComplete = true;
                earned += _bonusOnCompletion;
            }

            return earned;
        }

        public override string GetStatus()
        {
            return _isComplete ? "[X]" : $"[ ] Completed {_currentCount}/{_targetCount}";
        }

        public override string Serialize()
        {
            return $"Checklist|{Title}|{Description}|{Points}|{_targetCount}|{_currentCount}|{_bonusOnCompletion}";
        }
    }

    class Gamification
    {
        public int Score { get; private set; }
        public int Level { get; private set; }
        private HashSet<string> _badges = new HashSet<string>();

        public Gamification(int startingScore = 0)
        {
            Score = startingScore;
            Level = CalculateLevel();
        }

        public void AddPoints(int points)
        {
            if (points <= 0) return;
            Score += points;
            int oldLevel = Level;
            Level = CalculateLevel();

            if (Level > oldLevel)
            {
                Console.WriteLine($"\n*** Level up! You reached level {Level}! ***\n");
            }

            if (Score >= 100 && _badges.Add("Score100"))
                Console.WriteLine("Badge earned: 100+ points");
            if (Score >= 500 && _badges.Add("Score500"))
                Console.WriteLine("Badge earned: 500+ points");
        }

        public void RemovePoints(int p)
        {
            Score = Math.Max(0, Score - p);
            Level = CalculateLevel();
        }

        private int CalculateLevel()
        {
            return (Score / 100) + 1; // simple level formula
        }

        public void ShowStatus()
        {
            Console.WriteLine($"Score: {Score}  |  Level: {Level}");
            if (_badges.Count > 0)
                Console.WriteLine("Badges: " + string.Join(", ", _badges));
        }

        public string Serialize() => Score.ToString();
        public static Gamification Deserialize(string s)
        {
            if (int.TryParse(s, out int sc)) return new Gamification(sc);
            return new Gamification(0);
        }
    }

    class Program
    {
        static List<Goal> goals = new List<Goal>();
        static Gamification gamer = new Gamification(0);

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Eternal Quest!\n");
            bool running = true;

            while (running)
            {
                ShowMenu();
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        CreateGoal();
                        break;
                    case "2":
                        ListGoals();
                        break;
                    case "3":
                        RecordEvent();
                        break;
                    case "4":
                        ShowScore();
                        break;
                    case "5":
                        SaveToFile();
                        break;
                    case "6":
                        LoadFromFile();
                        break;
                    case "7":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.\n");
                        break;
                }
            }

            Console.WriteLine("Goodbye! Keep pressing forward on your Eternal Quest.");
        }

        static void ShowMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Create a new goal");
            Console.WriteLine("2. Show goals");
            Console.WriteLine("3. Record an event (complete a goal)");
            Console.WriteLine("4. Show score/level");
            Console.WriteLine("5. Save goals & score");
            Console.WriteLine("6. Load goals & score");
            Console.WriteLine("7. Exit\n");
        }

        static void CreateGoal()
        {
            Console.WriteLine("Select goal type:");
            Console.WriteLine("1. Simple goal (one-time)");
            Console.WriteLine("2. Eternal goal (repeatable)");
            Console.WriteLine("3. Checklist goal (repeat N times)");
            Console.Write("Choice: ");
            var t = Console.ReadLine();

            Console.Write("Enter title: ");
            var title = Console.ReadLine();
            Console.Write("Enter description: ");
            var desc = Console.ReadLine();
            int pts = ReadInt("Enter points awarded per event: ");

            switch (t)
            {
                case "1":
                    goals.Add(new SimpleGoal(title, desc, pts));
                    break;
                case "2":
                    goals.Add(new EternalGoal(title, desc, pts));
                    break;
                case "3":
                    int target = ReadInt("Enter how many times needed to complete: ");
                    int bonus = ReadInt("Enter bonus points awarded on completion: ");
                    goals.Add(new ChecklistGoal(title, desc, pts, target, bonus));
                    break;
                default:
                    Console.WriteLine("Unknown type. Aborting creation.\n");
                    return;
            }

            Console.WriteLine("Goal created.\n");
        }

        static void ListGoals()
        {
            if (!goals.Any())
            {
                Console.WriteLine("No goals yet.\n");
                return;
            }

            Console.WriteLine("Goals:");
            for (int i = 0; i < goals.Count; i++)
            {
                var g = goals[i];
                Console.WriteLine($"{i + 1}. {g.GetStatus()} {g.Title} - {g.Description}");
            }
            Console.WriteLine();
        }

        static void RecordEvent()
        {
            if (!goals.Any())
            {
                Console.WriteLine("No goals to record.\n");
                return;
            }

            ListGoals();
            int idx = ReadInt($"Choose a goal to record (1-{goals.Count}): ", 1, goals.Count) - 1;
            var goal = goals[idx];
            int gained = 0;
            try
            {
                gained = goal.RecordEvent();
            }
            catch
            {
                Console.WriteLine("Error recording event.\n");
                return;
            }

            if (gained > 0)
            {
                Console.WriteLine($"You earned {gained} points!\n");
                gamer.AddPoints(gained);
            }
            else
            {
                Console.WriteLine("No points earned (maybe the goal was already complete).\n");
            }
        }

        static void ShowScore()
        {
            gamer.ShowStatus();
            Console.WriteLine();
        }

        static void SaveToFile()
        {
            Console.Write("Enter filename to save (default EternalQuest.txt): ");
            var fn = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fn)) fn = "EternalQuest.txt";

            try
            {
                using (var sw = new StreamWriter(fn))
                {
                    // first line: score
                    sw.WriteLine(gamer.Serialize());

                    // goal lines
                    foreach (var g in goals)
                        sw.WriteLine(g.Serialize());
                }

                Console.WriteLine($"Saved to {fn}.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save: {ex.Message}\n");
            }
        }

        static void LoadFromFile()
        {
            Console.Write("Enter filename to load (default EternalQuest.txt): ");
            var fn = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fn)) fn = "EternalQuest.txt";

            if (!File.Exists(fn))
            {
                Console.WriteLine($"File '{fn}' not found.\n");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(fn).ToList();
                if (lines.Count == 0) { Console.WriteLine("File empty.\n"); return; }

                var scoreLine = lines[0];
                gamer = Gamification.Deserialize(scoreLine);

                var loaded = new List<Goal>();
                for (int i = 1; i < lines.Count; i++)
                {
                    var g = Goal.Deserialize(lines[i]);
                    if (g != null) loaded.Add(g);
                }

                goals = loaded;
                Console.WriteLine($"Loaded {goals.Count} goals and score from {fn}.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load: {ex.Message}\n");
            }
        }

        static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, out int value))
                {
                    if (value < min || value > max) Console.WriteLine($"Value must be between {min} and {max}.");
                    else return value;
                }
                else
                {
                    Console.WriteLine("Invalid number, try again.");
                }
            }
        }
    }
}
