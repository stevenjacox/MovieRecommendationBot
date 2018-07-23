using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieRecommendationBot
{
    public class Movies
    {
        public List<Movie> MovieList = new List<Movie>();

        public Movies()
        {
            MovieList.Add(new Movie { MovieName = "Training Day", Actors = new List<string> { "Denzel Washington", "Ethan Hawke" }, Genre = "Drama", Rating = 5, ReleaseYear = 2001 });
            MovieList.Add(new Movie { MovieName = "The Shawshank Redemption", Actors = new List<string> { "Tim Robbins", "Morgan Freeman" }, Genre = "Drama", Rating = 5, ReleaseYear = 1994 });
            MovieList.Add(new Movie { MovieName = "Dr. Strangelove or: How I Learned to Stop Worrying and Love the Bomb", Actors = new List<string> { "Peter Sellers", "George C. Scott" }, Genre = "Comedy", Rating = 3, ReleaseYear = 1964 });
        }

    }
    

    public class Movie
    {
        public string MovieName { get; set; }
        public string Genre { get; set; }
        public int ReleaseYear { get; set; }
        public List<string> Actors { get; set; }
        public double Rating { get; set; }
    }
}
