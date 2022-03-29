# SimpleJsonParser
A JSON Parser that is written in just about ~100 lines of code. The main code resides in JsonParser.cs and JsonStack.cs the rest is merely OOP routine.

Just recently during a job interview process, I was asked to provide a solution for a problem that required me to write a JSON (de)serializer using only C# 4.

I wrote the first working version within 3 hours, later on just out of curiosity I did a benchmark and learned that I have written the WORLD's FASTEST JSON (de)serializer.

Here are the results of doing 1000 deserializtion of a JSON:


- SimpleJsonParser: 5ms

- Newtonsoft: 63ms

- Jil: 12ms