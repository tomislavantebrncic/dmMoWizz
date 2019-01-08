﻿using dmMoWizz.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Services
{
    public class UserService
    {
        private UserRepository _usersRepository;
        private MoviesRepository _moviesRepository;

        public UserService()
        {
            _usersRepository = new UserRepository();
            _moviesRepository = new MoviesRepository();
        }

        public async System.Threading.Tasks.Task AddToWatchlistAsync(string id, int movieId)
        {
            var userInfo = _usersRepository.Get(id);

            userInfo.AddToWatchlist(movieId);

            await _usersRepository.Update(userInfo);
        }

        public async System.Threading.Tasks.Task RateAsync(string id, int movieId, int rating)
        {
            var userInfo = _usersRepository.Get(id);

            int incRating = rating;
            int incCount = 1;
            if (userInfo.IsRatedMovie(movieId))
            {
                incCount = 0;
                incRating -= userInfo.Ratings.First(mr => mr.MovieId == movieId).Rating;
            }

            userInfo.UpdateRatings(new Models.Mongo.MovieRating
            {
                MovieId = movieId,
                Rating = rating
            });

            _moviesRepository.UpdateRating(movieId, incRating, incCount);

            await _usersRepository.Update(userInfo);
        }
    }
}