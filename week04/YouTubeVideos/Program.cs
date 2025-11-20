using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        List<Video> videos = new List<Video>();

        // Video 1
        Video v1 = new Video("How to Cook Pasta Perfectly", "Chef Mario", 480);
        v1.AddComment(new Comment("Alice", "Thanks! This helped a lot!"));
        v1.AddComment(new Comment("John", "Mine came out perfect."));
        v1.AddComment(new Comment("Sarah", "Great tutorial!"));
        videos.Add(v1);

        // Video 2
        Video v2 = new Video("Beginner Guitar Lesson", "MusicWithTom", 720);
        v2.AddComment(new Comment("Jake", "Awesome lesson."));
        v2.AddComment(new Comment("Linda", "I finally learned my first chord!"));
        v2.AddComment(new Comment("Paul", "Very clear explanations."));
        videos.Add(v2);

        // Video 3
        Video v3 = new Video("Top 10 Travel Destinations", "WanderWorld", 600);
        v3.AddComment(new Comment("Emma", "Adding these to my bucket list!"));
        v3.AddComment(new Comment("Chris", "I’ve been to 3 of these!"));
        v3.AddComment(new Comment("Nina", "Beautiful places."));
        videos.Add(v3);

        // Display
        foreach (Video vid in videos)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Title: {vid.GetTitle()}");
            Console.WriteLine($"Author: {vid.GetAuthor()}");
            Console.WriteLine($"Length: {vid.GetLength()} seconds");
            Console.WriteLine($"Number of Comments: {vid.GetCommentCount()}");
            Console.WriteLine("Comments:");

            foreach (Comment c in vid.GetComments())
            {
                Console.WriteLine($" - {c.GetCommenterName()}: {c.GetText()}");
            }
        }

        Console.WriteLine("\nProgram finished.");
    }
}
