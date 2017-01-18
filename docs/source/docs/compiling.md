 - tagline: Fable takes F# to a whole new platform

**Attention**: This document corresponds to Fable 0.6.x and needs to be updated to the latest version. Please check the [migration guide](../blog/Introducing-0-7.html).

# Compiling to JavaScript

## Requirements

The following software needs to be installed in your machine:

- [Node.js](https://nodejs.org/): Fable is tested against 4.4, but latest stable version is recommended.
- .NET Framework/Mono: v4.5 or higher.

> In Windows, assembly resolution may not work correctly if [.NET Framework 4.5 SDK](https://developer.microsoft.com/en-us/windows/downloads/windows-8-sdk) is not installed.

> There's also a Fable version that runs on NetCore: [fable-compiler-netcore](https://www.npmjs.com/package/fable-compiler-netcore)

## Hello World Setup

Fable compiler and its core library (fable-core) are distributed through [npm](https://www.npmjs.com/),
Node's package manager, so you usually will have to set it up for most Fable projects. Create an empty
directory for your sample and within it, type:

```shell
npm init --yes
```

This will create a `package.json` file in the directory, which is used by npm to register your project
dependencies.

> It's also possible to use other alternative package managers that work with npm repository, like [yarn](https://yarnpkg.com/).

Now install `fable-compiler` and `fable-core` packages with the following command:

```shell
npm install --save fable-compiler fable-core
```

npm will download the packages and put them in a `node_modules` folder within the current directory.
The `--save` option tells npm to also register the dependencies in the `package.json` file (that way,
the next time you only need to type `npm install`). Please note that npm can also install packages and
commands globally, but it's recommended that you install `fable-compiler` always locally to make updates
easier and also to make it possible to use different versions of the compiler in different projects if
necessary.

We can already start writing some F# code. Honouring the _Hello World_ tradition, create a `hello_world.fsx`
file and write the following in it:

```fsharp
printfn "Hello World!"
```

> Fable can compile to JS most of FSharp.Core and a subset of .NET Base Class Library.
Check [Compatibility](compatibility.html) for more details.

Now we will compile the F# code to node and run it as a node app. Type the following:

```shell
node node_modules/fable-compiler hello_world.fsx --module commonjs
```

Fable compiler itself is a node app, so we use node to bootstrap it. After that we pass the compiler
arguments: the project file (in this case, a `.fsx` script) and the JS module target. When the compiler
finishes a new `hello_world.js` file should appear in the directory. You can also run this script using
node, printing `Hello World!` to your terminal.

```shell
node hello_world.js
```

## CLI options

Besides the default argument (`--projFile`), the following options are available:

Option                  | Short     | Description
------------------------|-----------|----------------------------------------------------------------------
`--outDir`              | `-o`      | Where to put compiled JS files. Defaults to project directory.
`--module`              | `-m`      | Specify module code generation: `commonjs`, `amd`, `umd` or `es2015` (default).
`--sourceMaps`          | `-s`      | Generate source maps: `false` (default), `true` or `inline`.
`--watch`               | `-w`      | Recompile project much faster on file modifications.
`--ecma`                |           | Specify ECMAScript target version: `es5` (default) or `es2015`.
`--verbose`             |           | Print more information about the compilation process.
`--symbols`             |           | F# symbols for conditional compilation, like `DEBUG`.
`--dll`                 |           | Generate a `.dll` assembly when creating libraries.
`--rollup`              |           | Bundle files and dependencies with Rollup.
`--includeJs`           |           | Compile with Babel and copy to `outDir` relative imports (starting with '.').
`--plugins`             |           | Paths to Fable plugins.
`--babelPlugins`        |           | Additional Babel plugins (without `babel-plugin-` prefix). Must be installed in the current directory.
`--refs`                |           | Alternative location for compiled JS files of referenced libraries.
`--coreLib`             |           | Shortcut for `--refs Fable.Core={VALUE}`.
`--loose`               |           | Enable “loose” transformations for babel-preset-es2015 plugins (true by default).
`--babelrc`             |           | Use a `.babelrc` file for Babel configuration (invalidates other Babel related options).
`--clamp`               |           | Compile unsigned byte arrays as Uint8ClampedArray.
`--noTypedArrays`       |           | Don't compile numeric arrays as JS typed arrays.
`--target`              | `-t`      | Use options from a specific target in `fableconfig.json`.
`--debug`               | `-d`      | Shortcut for `--target debug`.
`--production`          | `-p`      | Shortcut for `--target production`.
`--declaration`         |           | [EXPERIMENTAL] Generates corresponding ‘.d.ts’ file.
`--extra`               |           | Custom options for plugins in `{KEY}={VALUE}` format.
`--help`                | `-h`      | Display usage guide.

## Watch mode

TODO

## F# projects (.fsproj)

## Project references

You can use `--refs` argument to link referenced dll or projects with the JS import path that must be used,
using the following format: `[Reference name without extension]=[JS import path]`.

### Example: project reference

```shell
fable src/lib/MyLib.fsproj --outDir out/lib
fable src/main/MyProject.fsproj --refs MyLib=../lib
```

### Example: dll refence

We assume we have an npm package with the following structure:

```text
my-lib/
    js/MyLib.js
    bin/MyLib.dll
```

If we are referencing `node_modules/bin/MyLib.dll` in our project,
we can tell Fable to replace the refence with the JS code using the
argument below (note if we are using node or a bundler like Webpack
we can omit `./node_modules/` in the JS import path).

```shell
fable src/main/MyProject.fsproj --refs MyLib=my-lib/js
```

> See [fable-helpers-sample](https://www.npmjs.com/package/fable-helpers-sample) to know how to publish a Fable package.

TODO: Explain how to use the EntryModule attribute

## fableconfig.json

Rather than passing all the options to the CLI, it may be more convenient to put them
in JSON format in a file named `fableconfig.json` and let the compiler read them for you.
You can combine options from the CLI and `fableconfig.json`, when this happens the former will have preference.

To use `fableconfig.json` just pass the directory where the JSON file resides to Fable.
If you omit the path, the compiler will assume it's in the current directory.

```shell
fable my/path/
```

> Note that in this case, all path configurations (`projFile`, `outDir`, `plugins`...) will be
relative to the directory where `fableconfig.json` resides.

Project references can be passed using a plain object:

```shell
{
  "refs": {
    "MyLib": "../lib",
    "MyNs.AnotherProject": "../another"
  }
}
```

There are some options exclusive to `fableconfig.json`.

* **scripts**: Commands that should be executed during specific phases of compilation.
  Currently `prebuild`, `postbuild` and `postbuild-once` are accepted. For example, if you want
  to run tests defined in the npm `package.json` file after the build you can write.

```json
{
    "scripts": {
        "postbuild": "npm run test"
    }
}
```

> `postbuild` will run for every compilation in watch mode. If you only want
to run the script after the first full compilation, use `postbuild-once`.
Attention: On Windows, `postbuild-once` can only be used with executable files (`.exe`),
not with `.bat` or `.cmd` scripts.

> The scripts will run as if you typed the command on a terminal window from
the directory where `fableconfig.json` is. Fable scripts are not as powerful
as npm scripts in `package.json`: if you want to run a binary from a local
npm package you must specify the full path (e.g. use `node node_modules/webpack/bin/webpack`
instead of just `webpack`). But you can always call an npm script from `fableconfig.json`
as in the sample above.

* **targets**: You can group different options in targets. If you don't want,
  say, source maps when deploying for production, you can use a config file as
  seen below. When a target is specified, the options in the target will
  override the default ones. Activate the target by passing it to the CLI:
  `fable --target production`.


```json
{
    "sourceMaps": true,
    "targets": {
        "production": {
            "sourceMaps": false
        }
    }
}
```

When using a node `package.json` file, it's also possible to specify the minimum
version of Fable required to compile the project.

```json
{
    "engines": {
        "fable": "0.1.3"
    }
}
```


## fable-core

[Fable's core library](https://github.com/fable-compiler/Fable/blob/master/import/core/fable-core.js) must be included in the project.
When targeting node or using a module bundler you only need to add the dependency:

```shell
npm install --save fable-core
```

If targeting the browser and using AMD instead, you can load Fable's core lib with
[require.js](http://requirejs.org) as follows:

```html
<script src="node_modules/requirejs/require.js"></script>
<script>
requirejs.config({
    // Set the baseUrl to the path of the compiled JS code
    baseUrl: 'out',
    paths: {
        // Explicit path to core lib (relative to baseUrl, omit .js)
        'fable-core': '../node_modules/fable-core'
    }
});
// Load the entry file of the app (use array, omit .js)
requirejs(["app"]);
</script>
```


## Polyfill

When not using `--ecma es2015` or `--module es2015` (see below), after going through Babel pipeline
the code won't include any syntax foreign to ES5. However several ES2015 classes (like `Symbol`)
are used so it's advisable to include a polyfill like [core-js](https://github.com/zloirock/core-js)
to make sure the code works fine in any browser.

You can include the polyfill in a `script` tag in your HTML file before loading
the generated JS code like:

```html
<script src="node_modules/core-js/client/core.min.js"></script>
```

Or you can import it directly in your F# code if you're using a bundler like
Webpack or Browserify right before the entry point of your app.

```fsharp
open Fable.Core

JsInterop.importAll "core-js"
```

> The polyfill is not necessary when targeting node 4.4 or above.

> Babel includes [its own polyfill](http://babeljs.io/docs/usage/polyfill/)
with a lazy-sequence generator, but this is not needed as one is already included
in [fable-core.ts](https://github.com/fable-compiler/Fable/blob/master/src/fable/Fable.Core/npm/fable-core.ts).


## Modules

The compiler will keep the file structure of the F# project, wrapping each file in a [ES2015 module](https://github.com/lukehoban/es6features#modules).

According to the `--module` argument (see above), these modules can be transformed again by Babel to
[umd](https://github.com/umdjs/umd) (the default), [amd](http://requirejs.org/docs/whyamd.html), [commonjs](https://nodejs.org/docs/latest/api/modules.html), or not at all.

In the browser, when not using a bundler like Webpack or Browserify, you'll need a module loader like [require.js](http://requirejs.org) to start up the app.

When a F# file makes a reference to another, the compiler will create an [import statement](https://developer.mozilla.org/en/docs/web/javascript/reference/statements/import)
in the generated Javascript code. You can also generate imports by using the [Import attribute](interacting.html).

As JS must import external modules with an alias, there's no risk of namespace
collision so, for convenience, the compiler will use the minimum route to access
external objects. Meaning that if you have a F# file with one root module:

```fsharp
module MyNs1.MyNs2.MyModule

let myProperty = "Hello"
```

To access `myProperty` the generated code will import the file with an alias, say `$import0`,
and directly access the property from it: `$import0.myProperty`. The route has been eluded
as it's not necessary to prevent name conflicts. In the same way, if you have a file
with two modules:

```fsharp
namespace MyNs1.MyNs2

module MyModule1 =
    let myProperty = "Hello"

module MyModule2 =
    let myProperty = "Bye"
```

This time the compiler will omit the namespace but keep the F# module names,
as they're necessary to prevent name conflicts in the same file:

```js
$import0.MyModule1.myProperty !== $import0.MyModule2.myProperty
```

> Note: When referencing a module or type from another file, Fable will automatically create
the imports for the specific members you need. This allows [tree shaking](http://www.2ality.com/2015/12/webpack-tree-shaking.html)
but it also means using a `#load` directive in a script file just for the side effects
(for example, to run some code on the other file) won't work. Functions on the other
file must be called explicitly.

## Bundling

TODO: See http://fable.io/blog/Introducing-0-7.html#ES2015-Modules-and-Bundling

## Debugging

You can debug the generated JS code normally. Also, if you pass the `sourceMaps`
option to the compiler, it'll be possible to debug the F# code (with some limitations).
This is automatic for browser apps. For node, you'll need a tool like [node-inspector](https://github.com/node-inspector/node-inspector)
or a capable IDE. In the case of Visual Studio Code, you can find instructions [here](https://code.visualstudio.com/docs/editor/debugging)
(see Node Debugging > JavaScript Source Maps).

## Writing a library

Writing a library for Fable apps involves a couple of things more than normal projects.
You can fin a sample library project with a tutorial [here](https://github.com/fable-compiler/fable-helpers-sample).

## Plugins

Besides the compiler options, it's possible to add Fable and [Babel](https://github.com/thejameskyle/babel-handbook/blob/master/translations/en/README.md) plugins
to customize the build. Check [Plugins](plugins.html) to learn how to create your own Fable plugins.