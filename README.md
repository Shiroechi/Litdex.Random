# Litdex.Random

[Litdex.Random](https://github.com/Shiroechi/Litdex.Random) is continuation from [Litdex.Security.RNG](https://github.com/Shiroechi/Litdex.Security.RNG). Litdex.Random have different namespace and will become the focus of next development. Litdex.Security.RNG will still available but will not receive any bug fix or any update in the future.

Litdex.Random provide basic random generator function with extra feature.

Litdex.Random have already provide some basic random algorithm, so you can use it immediately rather than implement it yourself. But, still Litdex.Random created with extensibility in mind, so you can implement your own *random generator* with this library.

[![CodeFactor](https://www.codefactor.io/repository/github/shiroechi/litdex.random/badge)](https://www.codefactor.io/repository/github/shiroechi/litdex.random)

# Download

## Previous version

[![Nuget](https://img.shields.io/nuget/v/Litdex.Security.RNG?label=Litdex.Security.RNG&style=for-the-badge)](https://www.nuget.org/packages/Litdex.Security.RNG)

## Current Version

[![Nuget](https://img.shields.io/nuget/v/Litdex.Random?label=Litdex.Random&style=for-the-badge)](https://www.nuget.org/packages/Litdex.Random)

# This package contains:

Currently [Litdex.Random](https://github.com/Shiroechi/Litdex.Random) support this algorithm:

- [GJrand](http://gjrand.sourceforge.net/)
- [JSF](http://burtleburtle.net/bob/rand/smallprng.html)
- [Middle Square Weyl Sequence](https://en.wikipedia.org/wiki/Middle-square_method)
- [PCG](https://www.pcg-random.org/)
- [Romu](http://romu-random.org/)
- [Seiran](https://github.com/andanteyk/prng-seiran)
- SFC (Chris Doty-Humphrey's Chaotic PRNG)
- [Shioi](https://github.com/andanteyk/prng-shioi)
- [Shishua](https://github.com/espadrine/shishua)
- SplitMix64
- [Squares](<https://en.wikipedia.org/wiki/Counter-based_random_number_generator_(CBRNG)#Squares_RNG>)
- [Tyche](https://www.researchgate.net/publication/233997772_Fast_and_Small_Nonlinear_Pseudorandom_Number_Generators_for_Computer_Simulation)
- [Wyrand](https://github.com/wangyi-fudan/wyhash)
- [Xoroshiro and Xoshiro](http://prng.di.unimi.it/)

All of the algorithm have passing Practrand or Test01 test. But I've never test it individually, the author who is said that their algorithm past Practrand or Test01.

You can check in this website ["PRNG Battle Royale: 47 PRNGs � 9 consoles"](https://rhet.dev/wheel/rng-battle-royale-47-prngs-9-consoles/), the writer have tested some of the algorithm that have been implemented.

# How to use

For detailed use, read [How to use](https://github.com/Shiroechi/Litdex.Random/wiki/How-to-use)
or [Documentation](https://github.com/Shiroechi/Litdex.Random/wiki/Documentation)

The simple way to use

```C#
// create rng object
var rng = new Xoroshiro128plus();

// get random integer
var randomInt = rng.NextInt();
```

Want to create your own RNG?? Then read [Custom RNG](https://github.com/Shiroechi/Litdex.Random/wiki/Custom-RNG)

# Benchmark

## 32-bit RNG

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.1500 (1909/November2019Update/19H2)
AMD FX-8800P Radeon R7, 12 Compute Cores 4C+8G, 1 CPU, 4 logical and 4 physical cores
.NET SDK=6.0.300
  [Host]    : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
  MediumRun : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10  

```

|        Method |                 rand |      Mean |     Error |    StdDev |    Median |       Min |       Max | Ratio | RatioSD | Rank | Allocated |
|-------------- |--------------------- |----------:|----------:|----------:|----------:|----------:|----------:|------:|--------:|-----:|----------:|
| System.Random |                    ? |  6.573 ns | 0.5323 ns | 0.7802 ns |  6.698 ns |  5.593 ns |  7.526 ns |  1.00 |    0.00 |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt |           JSF 32-bit |  7.491 ns | 1.1079 ns | 1.6582 ns |  7.694 ns |  4.983 ns |  9.290 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt | JSF 3(...)otate [24] |  5.574 ns | 0.3391 ns | 0.4863 ns |  5.426 ns |  5.154 ns |  6.638 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt | Middl(...)uence [27] |  4.759 ns | 0.3152 ns | 0.4620 ns |  4.808 ns |  4.277 ns |  5.550 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt |    PCG XSH-RR 32-bit |  5.191 ns | 0.3312 ns | 0.4957 ns |  5.104 ns |  4.579 ns |  6.390 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt |    PCG XSH-RS 32-bit |  5.041 ns | 0.2313 ns | 0.3461 ns |  5.028 ns |  4.439 ns |  5.712 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt |     Romu Mono 32-bit |  4.660 ns | 0.1126 ns | 0.1615 ns |  4.626 ns |  4.358 ns |  4.963 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt |     Romu Quad 32-bit |  5.417 ns | 0.0755 ns | 0.1107 ns |  5.419 ns |  5.211 ns |  5.628 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt |     Romu Trio 32-bit |  6.159 ns | 0.8354 ns | 1.1435 ns |  6.885 ns |  4.887 ns |  7.807 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt |           SFC 32-bit |  5.767 ns | 0.0618 ns | 0.0906 ns |  5.743 ns |  5.576 ns |  5.910 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt |              Squares |  6.732 ns | 0.1325 ns | 0.1857 ns |  6.630 ns |  6.488 ns |  6.979 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt |                Tyche | 26.184 ns | 0.4675 ns | 0.6997 ns | 26.525 ns | 24.840 ns | 27.095 ns |     ? |       ? |    1 |         - |
|               |                      |           |           |           |           |           |           |       |         |      |           |
|       NextInt |              Tyche-i | 11.294 ns | 0.1009 ns | 0.1510 ns | 11.298 ns | 11.059 ns | 11.593 ns |     ? |       ? |    1 |         - |

## 64-bit RNG

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.18363.1500 (1909/November2019Update/19H2)
AMD FX-8800P Radeon R7, 12 Compute Cores 4C+8G, 1 CPU, 4 logical and 4 physical cores
.NET SDK=6.0.300
  [Host]    : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT
  MediumRun : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10 

```

|        Method |                rand |      Mean |     Error |    StdDev |    Median |       Min |       Max | Ratio | RatioSD | Rank |  Gen 0 | Allocated |
|-------------- |-------------------- |----------:|----------:|----------:|----------:|----------:|----------:|------:|--------:|-----:|-------:|----------:|
| System.Random |                   ? |  5.900 ns | 0.1165 ns | 0.1671 ns |  5.875 ns |  5.507 ns |  6.107 ns |  1.00 |    0.00 |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |              Gjrand | 15.280 ns | 2.0035 ns | 2.8087 ns | 17.052 ns | 11.979 ns | 18.086 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |          JSF 64-bit |  6.252 ns | 0.0755 ns | 0.1130 ns |  6.267 ns |  6.042 ns |  6.441 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong | PCG RXS-M-XS 64-bit |  6.041 ns | 0.2177 ns | 0.3191 ns |  6.048 ns |  5.498 ns |  6.537 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |     Romu Duo 64-bit |  5.384 ns | 0.0916 ns | 0.1371 ns |  5.440 ns |  5.180 ns |  5.586 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |  Romu Duo Jr 64-bit |  6.093 ns | 0.3680 ns | 0.5394 ns |  6.371 ns |  5.170 ns |  6.797 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |    Romu Quad 64-bit |  6.209 ns | 0.1032 ns | 0.1545 ns |  6.221 ns |  5.937 ns |  6.422 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |    Romu Trio 64-bit |  7.148 ns | 0.2255 ns | 0.3233 ns |  7.148 ns |  5.837 ns |  7.807 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |          SFC 64-bit |  5.792 ns | 0.0870 ns | 0.1191 ns |  5.743 ns |  5.652 ns |  6.009 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |              Seiran |  5.353 ns | 0.0896 ns | 0.1341 ns |  5.299 ns |  5.173 ns |  5.581 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |               Shioi |  5.402 ns | 0.0978 ns | 0.1370 ns |  5.406 ns |  5.179 ns |  5.755 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |             Shishua | 19.109 ns | 0.4884 ns | 0.7310 ns | 19.153 ns | 17.952 ns | 20.504 ns |     ? |       ? |    1 | 0.0306 |      16 B |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |          SplitMix64 |  5.715 ns | 0.0902 ns | 0.1349 ns |  5.778 ns |  5.479 ns |  5.925 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |              Wyrand |  7.789 ns | 0.4183 ns | 0.6260 ns |  7.433 ns |  7.254 ns |  9.696 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |     Xoroshiro 128** |  5.309 ns | 0.0940 ns | 0.1318 ns |  5.305 ns |  5.138 ns |  5.652 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |      Xoroshiro 128+ |  5.448 ns | 0.1547 ns | 0.2315 ns |  5.353 ns |  5.097 ns |  5.954 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |     Xoroshiro 128++ |  5.389 ns | 0.0832 ns | 0.1246 ns |  5.418 ns |  5.170 ns |  5.585 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |       Xoshiro 256** |  6.104 ns | 0.1472 ns | 0.2063 ns |  6.159 ns |  5.765 ns |  6.401 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |        Xoshiro 256+ |  5.800 ns | 0.0493 ns | 0.0708 ns |  5.784 ns |  5.650 ns |  5.929 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |       Xoshiro 256++ |  6.192 ns | 0.0626 ns | 0.0878 ns |  6.181 ns |  6.060 ns |  6.384 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |       Xoshiro 512** | 10.045 ns | 0.4068 ns | 0.6089 ns | 10.123 ns |  8.687 ns | 11.079 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |        Xoshiro 512+ |  8.907 ns | 0.1249 ns | 0.1869 ns |  8.922 ns |  8.650 ns |  9.361 ns |     ? |       ? |    1 |      - |         - |
|               |                     |           |           |           |           |           |           |       |         |      |        |           |
|      NextLong |       Xoshiro 512++ |  9.383 ns | 0.4581 ns | 0.6856 ns |  9.100 ns |  8.706 ns | 10.644 ns |     ? |       ? |    1 |      - |         - |

# Warning

For method that generate an arbitary byte array, using method `NextBytes` or `Fill`.

```C#
var rng = new Xoroshiro128plus();
var bytes = rng.NextBytes(10);
```

Litdex.Random generate the array using multiple `uint` or `ulong`, each `uint` or `ulong` will converted into byte array. When converted to byte array, Litdex.Random order the byte based on Little Endian. Then each converted byte array will be concated with other byte array.

# Contribute

Feel free to open new [issue](https://github.com/Shiroechi/Litdex.Random/issues) or [PR](https://github.com/Shiroechi/Litdex.Random/pulls).

# Donation

Like this library? Please consider donation

[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/X8X81SP2L)
