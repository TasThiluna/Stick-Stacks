using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using rnd = UnityEngine.Random;
using System.Threading;

public class stickStacks : MonoBehaviour
{
    public new KMAudio audio;
    public KMBombInfo bomb;
    public KMBombModule module;

    private int stackUsed;

    private static readonly string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly int[][][] stickArrays = new int[][][]
    {
        new int[][]
        {
            new int[] { 0, 5, 10, 3, 4 },
            new int[] { 5, 6, 15, 8, 9 },
            new int[] { 10, 11, 12, 13, 20 },
            new int[] { 15, 12, 17, 18, 24 },
            new int[] { 20, 21, 22, 23, 24 }
        },
        new int[][]
        {
            new int[] { 0 },
            new int[] { 0 },
            new int[] { 0 },
            new int[] { 0 },
            new int[] { 0 }
        },
        new int[][]
        {
            new int[] { 0 },
            new int[] { 0 },
            new int[] { 0 },
            new int[] { 0 },
            new int[] { 0 }
        },
        new int[][]
        {
            new int[] { 0 },
            new int[] { 0 },
            new int[] { 0 },
            new int[] { 0 },
            new int[] { 0 }
        }
    };

    private static int moduleIdCounter = 1;
    private int moduleId;
    private bool moduleSolved;

    private void Awake()
    {
        moduleId = moduleIdCounter++;
    }

    private void Start()
    {
        if (bomb.GetBatteryCount(Battery.AA) != 0 && bomb.GetBatteryCount(Battery.D) != 0)
            stackUsed = 0;
        else if (bomb.GetBatteryCount(Battery.AA) != 0)
            stackUsed = 1;
        else if (bomb.GetBatteryCount(Battery.D) != 0)
            stackUsed = 2;
        else
            stackUsed = 3;
        stackUsed = 0; // TEMP

        StartCoroutine(GeneratePuzzle());
    }

    private IEnumerator GeneratePuzzle()
    {
        var combinations = Permute(new int[] { 0, 1, 2, 3, 4 });
        var threadDone = false;
        var thread = new Thread(() =>
        {
            var random = new System.Random();
        tryAgain:
            var letters = new string[5];
            for (int i = 0; i < 5; i++)
                letters[i] = new string(Enumerable.Repeat(alphabet, 5).Select(x => PickRandom(x, random)).ToArray());
            if (combinations.Count(x => ValidStack(x, letters)) == 0)
                goto tryAgain;
            threadDone = true;
            return;
        });
        thread.Start();
        while (!threadDone)
            yield return null;
        Debug.Log("Puzzle generated!");
    }

    private bool ValidStack(List<int> combination, string[] originalLetters)
    {
        var letters = originalLetters.ToArray();
        var a = letters[0];
        var b = letters[1];
        var c = letters[2];
        var d = letters[3];
        var e = letters[4];
        letters[combination.IndexOf(0)] = a;
        letters[combination.IndexOf(1)] = b;
        letters[combination.IndexOf(2)] = c;
        letters[combination.IndexOf(3)] = d;
        letters[combination.IndexOf(4)] = e;
        var allLetters = letters.Join("");
        var wordsMade = new List<string>();
        var usedArrays = stickArrays[stackUsed];
        for (int i = 0; i < 5; i++)
            wordsMade.Add(new string(usedArrays[i].Select(x => allLetters[x]).ToArray()));
        return wordsMade.All(x => wordList.Phrases.Contains(x));
    }

    private static List<List<int>> Permute(int[] nums)
    {
        var list = new List<List<int>>();
        return DoPermute(nums, 0, nums.Length - 1, list);
    }

    private static List<List<int>> DoPermute(int[] nums, int start, int end, List<List<int>> list)
    {
        if (start == end)
            list.Add(new List<int>(nums));
        else
        {
            for (int i = start; i <= end; i++)
            {
                Swap(ref nums[start], ref nums[i]);
                DoPermute(nums, start + 1, end, list);
                Swap(ref nums[start], ref nums[i]);
            }
        }
        return list;
    }

    private static void Swap(ref int a, ref int b)
    {
        var temp = a;
        a = b;
        b = temp;
    }

    private static T PickRandom<T>(IEnumerable<T> src, System.Random rnd)
    {
        var list = (src as IList<T>) ?? src.ToArray();
        if (list.Count == 0)
            throw new InvalidOperationException("Cannot pick an element from an empty set.");
        return list[rnd.Next(0, list.Count)];
    }

    // Twitch Plays
#pragma warning disable 414
    private readonly string TwitchHelpMessage = "!{0} ";
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string input)
    {
        yield return null;
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
    }
}
