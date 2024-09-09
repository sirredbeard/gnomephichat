# GNOME Phi Chat

Simple .NET LLM chat application.

Uses:
* .NET 8
* GTK#
* ONNX Runtime GenAI

Model: Phi-3-mini-4k-instruct-onnx

Model variants:
* CPU-based
* DirectML-accelerated (for use on Windows Subsystem for Linux)

## Download models and copy into place:

```
huggingface-cli download microsoft/Phi-3-mini-4k-instruct-onnx --include cpu_and_mobile/cpu-int4-rtn-block-32-acc-level-4/* --local-dir Phi-3-mini-4k-instruct-onnx/cpu-int4-rtn-block-32-acc-level-4/

huggingface-cli download microsoft/Phi-3-mini-4k-instruct-onnx --include directml/directml-int4-awq-block-128/* --local-dir Phi-3-mini-4k-instruct-onnx/directml-int4-awq-block-128/
```