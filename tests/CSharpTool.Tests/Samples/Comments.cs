// Copyright 2024 Example Contributors
// Licensed under the Apache License, Version 2.0

using System;

namespace Example;

/// <summary>
/// Adds two numbers together.
/// </summary>
public class Calculator
{
    // ok
    public int Add(int a, int b)
    {
        // add the two numbers and return the result
        return a + b;
    }

    /* a block comment that explains nothing */
    public int Subtract(int a, int b) => a - b;

    // ReSharper disable once UnusedMember.Global
    public int Zero() => 0;
}
