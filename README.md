# GNOME Phi Chat

Simple .NET LLM chat application.

![image](https://github.com/user-attachments/assets/e75189ba-cbb7-4e18-b58a-1a99703d48cb)

Uses:
* .NET 8
* GTK# for Linux and Windows (bundles GTK# Runtime for Windows)
* [ONNX Runtime GenAI](https://github.com/microsoft/onnxruntime-genai)

Model: Phi-3-mini-4k-instruct-onnx

Model variants:
* CPU-based
* DirectML-accelerated (for use on Windows and Windows Subsystem for Linux)

## Download models and copy into place:

```
huggingface-cli download microsoft/Phi-3-mini-4k-instruct-onnx --include cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/* --local-dir Phi-3-mini-4k-instruct-onnx/cpu-int4-rtn-block-32-acc-level-4/

huggingface-cli download microsoft/Phi-3-mini-4k-instruct-onnx --include directml/directml-int4-awq-block-128/* --local-dir Phi-3-mini-4k-instruct-onnx/directml-int4-awq-block-128/
```

## Restore

```
dotnet restore
dotnet build
dotnet run
```

## Notes

DirectML models currently throwing a message about DML not being available. Can be fixed by setting runtime target in csproj to win-x64 (see https://github.com/microsoft/onnxruntime-genai/issues/862) but working on how to address when multiple targets are listed.
