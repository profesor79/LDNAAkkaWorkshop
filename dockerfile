FROM microsoft/windowsservercore
ADD publish/ /
ENTRYPOINT Profesor79.Merge.Consoler.exe
