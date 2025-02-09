﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using RecipesEFCore.DataAccess.SQLServer;
using RecipesEFCore3.Models;
using RecipesEFCore3.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace RecipesEFCore3.Endpoints
{
    public static class RecipeEndpoints
    {
        public static void MapRecipeEndpoints(this WebApplication app)
        {
            app.MapPost("/recipes", async (IRecipeService recipeService, RecipeDto recipeDto)
                => await recipeService.CreateRecipeAsync(recipeDto));

            app.MapGet("/recipes", async (IRecipeService recipeService, int? page, int? pageSize)
                => await recipeService.GetRecipesAsync(page ?? 1, pageSize ?? 50));

            app.MapGet("/recipes/{id}", async (IRecipeService recipeService, int id)
                => await recipeService.GetRecipeByIdAsync(id));

            app.MapGet("/recipes/search", async (IRecipeService recipeService, string query, int? page, int? pageSize)
                => await recipeService.SearchRecipesAsync(query, page ?? 1, pageSize ?? 50));

            app.MapPut("/recipes/{id}", async (IRecipeService recipeService, int id, RecipeDto recipeDto)
                => await recipeService.UpdateRecipeAsync(id, recipeDto));

            app.MapDelete("/recipes/{id}", async (IRecipeService recipeService, int id)
                => await recipeService.DeleteRecipeAsync(id));

        }

        //private static bool TryValidateModel(object model, out List<ValidationResult> results)
        //{
        //    var context = new ValidationContext(model, serviceProvider: null, items: null);
        //    results = new List<ValidationResult>();

        //    return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        //}

        //private static async Task<IResult> CreateRecipe(RecipesEFCoreDbContext dbContext, IMapper mapper, RecipeDto recipeDto)
        //{
        //    // Валидация входных данных
        //    if (!TryValidateModel(recipeDto, out var errors))
        //    {
        //        return Results.BadRequest(errors);
        //    }

        //    // Маппинг RecipeDto на Recipe (без ингредиентов)
        //    var recipe = mapper.Map<Recipe>(recipeDto);

        //    foreach (var ingredientDto in recipeDto.Ingredients)
        //    {
        //        // Проверяем, существует ли ингредиент в базе данных
        //        var ingredient = await dbContext.Ingredients
        //            .FirstOrDefaultAsync(i => i.Name == ingredientDto.Name);

        //        if (ingredient == null)
        //        {
        //            // Если ингредиент не существует, создаём новый
        //            ingredient = new Ingredient { Name = ingredientDto.Name };
        //            dbContext.Ingredients.Add(ingredient);
        //            await dbContext.SaveChangesAsync();
        //        }

        //        // Добавляем связь в RecipeIngredients
        //        recipe.RecipeIngredients.Add(new RecipeIngredient
        //        {
        //            IngredientId = ingredient.IngredientId,
        //            Quantity = ingredientDto.Quantity,
        //            Unit = ingredientDto.Unit
        //        });
        //    }

        //    await dbContext.Recipes.AddAsync(recipe);
        //    await dbContext.SaveChangesAsync();

        //    // Формируем ответ с помощью DTO
        //    var recipeResponseDto = new RecipeResponseDto
        //    {
        //        RecipeID = recipe.RecipeID,
        //        Name = recipe.Name,
        //        IsVegetarian = recipe.IsVegetarian,
        //        IsVegan = recipe.IsVegan,
        //        Ingredients = recipe.RecipeIngredients.Select(ri => new IngredientDto
        //        {
        //            Name = ri.Ingredient.Name,
        //            Quantity = ri.Quantity,
        //            Unit = ri.Unit
        //        }).ToList()
        //    };

        //    return Results.Created($"/recipes/{recipe.RecipeID}", recipeResponseDto);
        //}


        //private static async Task<IResult> GetRecipeList(RecipesEFCoreDbContext dbContext, IMapper mapper)
        //{
        //    var recipes = await dbContext.Recipes
        //            .Include(r => r.RecipeIngredients)
        //                .ThenInclude(ri => ri.Ingredient)
        //            .ToListAsync();

        //    var recipeResponseDtos = mapper.Map<List<RecipeResponseDto>>(recipes);
        //    return Results.Ok(recipeResponseDtos);
        //}

        //private static async Task<IResult> GetRecipeById(RecipesEFCoreDbContext dbContext, IMapper mapper, int id)
        //{
        //    var recipes = await dbContext.Recipes
        //        .Include(r => r.RecipeIngredients)
        //            .ThenInclude(ri => ri.Ingredient)
        //        .FirstOrDefaultAsync(r => r.RecipeID == id);

        //    if (recipes == null)
        //    {
        //        return Results.NotFound();
        //    }

        //    var recipeResponseDto = mapper.Map<RecipeResponseDto>(recipes);
        //    return Results.Ok(recipeResponseDto);
        //}

        //private static async Task<IResult> GetRecipeFilter(RecipesEFCoreDbContext dbContext, IMapper mapper, string query)
        //{
        //    var recipes = await dbContext.Recipes
        //        .Include(r => r.RecipeIngredients)
        //            .ThenInclude(ri => ri.Ingredient)
        //        .Where(r => r.Name.ToLower().Contains(query.ToLower()) || r.RecipeIngredients.Any(ri => ri.Ingredient.Name.ToLower().Contains(query.ToLower())))
        //        .ToListAsync();

        //    var recipeResponseDto = mapper.Map<List<RecipeResponseDto>>(recipes);
        //    return Results.Ok(recipeResponseDto);
        //}


        //private static async Task<IResult> UpdateRecipe(RecipesEFCoreDbContext dbContext, IMapper mapper, int id, RecipeDto recipeDto)
        //{
        //    // Поиск рецепта в базе данных
        //    var recipe = await dbContext.Recipes
        //        .Include(r => r.RecipeIngredients)
        //        .ThenInclude(ri => ri.Ingredient)
        //        .FirstOrDefaultAsync(r => r.RecipeID == id);

        //    if (recipe == null)
        //    {
        //        return Results.NotFound();
        //    }

        //    // Маппинг RecipeDto на существующий Recipe (без ингредиентов)
        //    mapper.Map(recipeDto, recipe);

        //    // Удаляем старые связи между рецептом и ингредиентами
        //    dbContext.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);

        //    // Добавляем новые связи из DTO
        //    foreach (var ingredientDto in recipeDto.Ingredients)
        //    {
        //        // Проверяем, существует ли ингредиент в базе данных
        //        var ingredient = await dbContext.Ingredients
        //            .FirstOrDefaultAsync(i => i.Name == ingredientDto.Name);

        //        if (ingredient == null)
        //        {
        //            // Если ингредиент не существует, создаём новый
        //            ingredient = new Ingredient { Name = ingredientDto.Name };
        //            dbContext.Ingredients.Add(ingredient);
        //            await dbContext.SaveChangesAsync();
        //        }

        //        // Добавляем новую связь в RecipeIngredients
        //        recipe.RecipeIngredients.Add(new RecipeIngredient
        //        {
        //            IngredientId = ingredient.IngredientId,
        //            Quantity = ingredientDto.Quantity,
        //            Unit = ingredientDto.Unit
        //        });
        //    }

        //    await dbContext.SaveChangesAsync();

        //    return Results.Ok();
        //}

        //private static async Task<IResult> DeleteRecipe(RecipesEFCoreDbContext dbContext, int id)
        //{
        //    var recipe = await dbContext.Recipes
        //        .Include(r => r.RecipeIngredients)
        //        .FirstOrDefaultAsync(r => r.RecipeID == id);

        //    if (recipe == null)
        //    {
        //        return Results.NotFound();
        //    }

        //    // Удаляем все связи ингредиентов с рецептом
        //    dbContext.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);

        //    // Удаляем сам рецепт
        //    dbContext.Recipes.Remove(recipe);

        //    await dbContext.SaveChangesAsync();

        //    return Results.Ok();
        //}
    }
}