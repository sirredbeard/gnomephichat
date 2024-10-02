# GNOME Phi Chat

Simple GTK3 LLM chat application written using .NET powered by Phi-3-mini.

![image](https://github.com/user-attachments/assets/e75189ba-cbb7-4e18-b58a-1a99703d48cb)

Uses:
* .NET 9
* GTK# for Linux and Windows (bundles GTK# Runtime for Windows)
* [ONNX Runtime GenAI](https://github.com/microsoft/onnxruntime-genai)


## Download model and copy into place:

```
huggingface-cli download microsoft/Phi-3-mini-4k-instruct-onnx --include cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/* --local-dir Phi-3-mini-4k-instruct-onnx/cpu-int4-rtn-block-32-acc-level-4/

```

## Build

```
dotnet restore
dotnet build
dotnet run
```