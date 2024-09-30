using System;
using System.Collections.Generic;
using System.IO;
using Gtk;
using Microsoft.ML.OnnxRuntimeGenAI;

class Program
{
    static void Main(string[] args)
    {
        Application.Init();
        var win = new Window("AI Assistant");
        win.SetDefaultSize(800, 600);
        win.DeleteEvent += (o, e) => Application.Quit();

        var vbox = new VBox();
        var textView = new TextView();
        textView.Editable = false;
        var textEntry = new Entry();
        var sendButton = new Button("Send");
        var spinner = new Spinner();

        vbox.PackStart(new ScrolledWindow { textView }, true, true, 0);
        vbox.PackStart(textEntry, false, false, 0);
        vbox.PackStart(sendButton, false, false, 0);
        vbox.PackStart(spinner, false, false, 0);
        win.Add(vbox);

        var modelPath = GetModelPath(2); // Change the index to select the desired model
        var model = new Model(modelPath);
        var tokenizer = new Tokenizer(model);

        var systemPrompt = "You are an AI assistant that helps people find information. Answer questions using a direct style. Do not share more information than requested by the users.";

        EventHandler submitEntry = (sender, e) =>
        {
            var userQ = textEntry.Text;
            if (string.IsNullOrEmpty(userQ))
            {
                Application.Quit();
                return;
            }

            textView.Buffer.Text += $"Q: {userQ}\n";
            textEntry.Text = string.Empty;

            spinner.Start();

            var fullPrompt = $"<|system|>{systemPrompt}<|end|><|user|>{userQ}<|end|><|assistant|>";
            var tokens = tokenizer.Encode(fullPrompt);

            var generatorParams = new GeneratorParams(model);
            generatorParams.SetSearchOption("max_length", 2048);
            generatorParams.SetSearchOption("past_present_share_buffer", true);
            generatorParams.SetInputSequences(tokens);

            var generator = new Generator(model, generatorParams);
            var outputTokens = new List<int>();

            while (!generator.IsDone())
            {
                generator.ComputeLogits();
                generator.GenerateNextToken();
                var newToken = generator.GetSequence(0).Slice(generator.GetSequence(0).Length - 1, 1);
                outputTokens.AddRange(newToken);
            }

            var output = tokenizer.Decode(outputTokens.ToArray());
            textView.Buffer.Text += $"Phi3: {output}\n";

            spinner.Stop();
        };

        textEntry.Activated += submitEntry;
        sendButton.Clicked += submitEntry;

        win.ShowAll();
        Application.Run();
    }

    static string GetModelPath(int selectedIndex)
    {
        if (selectedIndex == 1) // DirectML-powered Phi-3-mini
        {
            return Path.Combine(AppContext.BaseDirectory, @"Phi-3-mini-4k-instruct-onnx/directml-int4-awq-block-128");
        }
        else if (selectedIndex == 2) // CUDA-powered Phi-3-mini
        {
            return Path.Combine(AppContext.BaseDirectory, @"Phi-3-mini-4k-instruct-onnx/cuda-int4-rtn-block-32");
        }
        else // CPU-powered Phi-3-mini
        {
            return Path.Combine(AppContext.BaseDirectory, @"Phi-3-mini-4k-instruct-onnx/cpu-int4-rtn-block-32-acc-level-4");
        }
    }
}