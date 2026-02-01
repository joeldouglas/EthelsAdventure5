using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static lib_DialogueLines;

public static class DialogueCsvParser
{

    public static List<DialogueLine> LoadFromCsvText(string csvText)
    {
        var result = new List<DialogueLine>();

        // Parse all rows
        List<List<string>> rows = ParseCsv(csvText);

        if (rows.Count == 0) return result;

        // Skip header row
        for (int r = 1; r < rows.Count; r++)
        {
            var row = rows[r];
            if (row == null || row.Count == 0) continue;

            
            if (row.Count < 3) continue;

            // 0 = ID
            if (!int.TryParse(row[0].Trim(), out int id))
                continue;

            // 1 = SPEAKER
            string speaker = (row[1] ?? "").Trim();

            // 2 = LINE            
            string line = (row[2] ?? "").Trim();

            result.Add(new DialogueLine(id, speaker, line));
        }

        return result;
    }



    private static List<List<string>> ParseCsv(string text)
    {
        var rows = new List<List<string>>();
        var currentRow = new List<string>();
        var field = new StringBuilder();

        bool inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // skip escape quotes
                    bool hasNext = (i + 1) < text.Length;
                    if (hasNext && text[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {                        
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    currentRow.Add(field.ToString());
                    field.Clear();
                }
                else if (c == '\r')
                {
                    // ignore
                }
                else if (c == '\n')
                {
                    currentRow.Add(field.ToString());
                    field.Clear();

                    // yeet trailing rows
                    rows.Add(currentRow);
                    currentRow = new List<string>();
                }
                else
                {
                    field.Append(c);
                }
            }
        }

        // if file doesnt end with newline
        currentRow.Add(field.ToString());
        if (currentRow.Count > 1 || (currentRow.Count == 1 && !string.IsNullOrEmpty(currentRow[0])))
            rows.Add(currentRow);

        return rows;
    }
}
