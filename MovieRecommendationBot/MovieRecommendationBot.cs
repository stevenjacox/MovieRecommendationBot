using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Recognizers.Text;

namespace MovieRecommendationBot
{
    public class MovieRecommendationBot : IBot
    {

        private readonly DialogSet dialogs;

        static string movieType;
        static string favoriteActor;

        Movies movies = new Movies();

        public MovieRecommendationBot()
        {
            dialogs = new DialogSet();
            // Define our dialog
            dialogs.Add("movieRecommendation", new WaterfallStep[]
            {
        async (dc, args, next) =>
        {
            
            await dc.Context.SendActivity("Welcome to the movie recommendation service.");

            await dc.Prompt("movieTypePrompt", "What type of movie are you looking for?");
        },
        async(dc, args, next) =>
        {
            movieType = args["Text"].ToString().ToLowerInvariant();
            
            // Ask for next info
            await dc.Prompt("favoriteActorPrompt", "Who is your favorite actor?");

        },
        async(dc, args, next) =>
        {
            favoriteActor = args["Text"].ToString().ToLowerInvariant();

            // Ask for next info
            await dc.Prompt("ratingPrompt", "What is the least number of stars you would want in this movie?");
        },
        async(dc, args, next) =>
        {
            Movie movieMatch;
            movieMatch = movies.MovieList.Where (movie => (movie.Genre.ToLowerInvariant().Equals(movieType)
            && movie.Actors.Contains(favoriteActor.ToLowerInvariant()))
            && movie.Rating > (int)args["Value"]
            ).FirstOrDefault();

            if (movieMatch == null)
            {
                movieMatch = movies.MovieList.Where (movie => (movie.Genre.ToLowerInvariant().Equals(movieType)
            || movie.Actors.Contains(favoriteActor.ToLowerInvariant()))
            && movie.Rating > (int)args["Value"]
            ).FirstOrDefault();
                
            }

            if (movieMatch != null)
            {
                string msg = "I found a movie that you might want to check out. " +
                $"\nMovie Name: {movieMatch.MovieName.ToString()} " +
                $"\nRelease Year: {movieMatch.ReleaseYear.ToString()} " +
                $"\nGenre: {movieMatch.Genre.ToString()} " +
                $"\nRating: {movieMatch.Rating} " +
                $"\nActors: {string.Join(", ", movieMatch.Actors.ToArray())}";
                await dc.Context.SendActivity(msg);
                await dc.End();
            }
            else
            {

                Movie goodMovie = movies.MovieList.Where(movie => movie.Rating >= (int)args["Value"]).First();

                if (goodMovie == null)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(0, movies.MovieList.Count() -1);

                    //Random movie becomes good movie when you don't have a match
                    goodMovie = movies.MovieList.ElementAt(index);

                }

                string msg = "I couldn't find an exact match for you but may I recommend the following: " +
                $"\nMovie Name: {goodMovie.MovieName.ToString()} " +
                $"\nRelease Year: {goodMovie.ReleaseYear.ToString()} " +
                $"\nGenre: {goodMovie.Genre.ToString()} " +
                $"\nRating: {goodMovie.Rating}" +
                $"\nActors: {string.Join(",", goodMovie.Actors.ToArray())}";
                await dc.Context.SendActivity(msg);
                await dc.End();

            }

        }
            });

            // Add a prompt for the movie type
            dialogs.Add("movieTypePrompt", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
            // Add a prompt for a favorite actor
            dialogs.Add("favoriteActorPrompt", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
            // Add a prompt for a movie's rating
            dialogs.Add("ratingPrompt", new Microsoft.Bot.Builder.Dialogs.NumberPrompt<int>(Culture.English));
        }

        public async Task OnTurn(ITurnContext context)
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // The type parameter PropertyBag inherits from 
                // Dictionary<string,object>
                var state = ConversationState<Dictionary<string, object>>.Get(context);
                var dc = dialogs.CreateContext(context, state);
                await dc.Continue();

                // Additional logic can be added to enter each dialog depending on the message received
                string activityText = context.Activity.Text.ToLowerInvariant();

                if (!context.Responded)
                {
                    if (activityText.Contains("recommend") || activityText.Contains("movie"))
                    {
                        await dc.Begin("movieRecommendation");
                    }
                    else
                    {
                        await context.SendActivity($"You said '{context.Activity.Text}'; maybe you could ask me to recommend you movie?");
                    }
                }
            }
        }
    }
}
