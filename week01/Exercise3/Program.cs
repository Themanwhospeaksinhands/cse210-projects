using System;

class Program
{
    static void Main(string[] args)
    {
        
        Random rng = new Random();
        int magicNumber = rng.Next(1, 101);

        int guess = -1;

        while (guess != magicNumber)
        {
            Console.Write("What is your guess for the magic number? ");
            guess = int.Parse(Console.ReadLine());

            if (magicNumber > guess) {
                Console.WriteLine("Higher");
            } else if (magicNumber < guess) {
                Console.WriteLine("Lower");
            } else {
                Console.WriteLine("You guessed it!");
            }

        }                    
    }
}
