﻿using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Attributes;

public class FutureDateAttribute : ValidationAttribute
{
    public FutureDateAttribute()
    {
        ErrorMessage = "The field 'dueDate' must be a future date.";
    }

    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            return date > DateTime.Now;
        }
        return true;
    }
}
