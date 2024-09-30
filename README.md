# GNOME Phi Chat

Simple GTK3 LLM chat application written using .NET powered by Phi-3-mini.

![image](https://github.com/user-attachments/assets/e75189ba-cbb7-4e18-b58a-1a99703d48cb)

Uses:
* .NET 9
* GTK# for Linux and Windows (bundles GTK# Runtime for Windows)
* [ONNX Runtime GenAI](https://github.com/microsoft/onnxruntime-genai)

Requires:
* GTK 3
* .NET 9 SDK to build
* Download models from Huggingface (see below)
* x64-only currently

Model variants:
* CPU-based
* NVIDIA CUDA-accelerated
* DirectML-accelerated (for use on Windows and Windows Subsystem for Linux)*

## Download models and copy into place:

```
huggingface-cli download microsoft/Phi-3-mini-4k-instruct-onnx --include cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/* --local-dir Phi-3-mini-4k-instruct-onnx/cpu-int4-rtn-block-32-acc-level-4/

huggingface-cli download microsoft/Phi-3-mini-4k-instruct-onnx --include cuda/cud
a-int4-rtn-block-32/* --local-dir Phi-3-mini-4k-instruct-onnx/cuda-int4-rtn-block-32/

huggingface-cli download microsoft/Phi-3-mini-4k-instruct-onnx --include directml/directml-int4-awq-block-128/* --local-dir Phi-3-mini-4k-instruct-onnx/directml-int4-awq-block-128/
```

## Restore

```
dotnet restore
dotnet build
dotnet run
```

## Notes

*DirectML model is currently throwing a message about DML not being available. Can be fixed by setting runtime target in csproj to win-x64 ONLY (see https://github.com/microsoft/onnxruntime-genai/issues/862) but working on how to address when multiple targets are listed.
