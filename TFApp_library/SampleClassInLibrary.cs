﻿namespace TFApp_library;

public class SampleClassInLibrary
{
    // public static string Message = "(updated) 12Factor App sample library message!";

    private static readonly string SAMPLE_MESSAGE = "(updated) 12Factor App sample library message!";
    
    public string Message => SAMPLE_MESSAGE;
}