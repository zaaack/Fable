 - tagline: Fable takes F# to a whole new platform

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

Fable compilations has a warmup time of around five seconds, and big projects can take longer to compile.
This can break the workflow of web developers, who have become used to see their code running on the browser
right after editing a file.

The `--watch` mode is available to achieve a similar experience with Fable. Using this mode the
first compilation will still take some time but after that Fable will only compile the edited files
resulting in much faster build times. By activating the `--watch` flag, after the first compilation
Fable will watch the project directory for changes in the source files.

`--watch` can also accept a string (or a string array) to indicate the directories that must be watched.
This is useful when the `.fs` source files are not in the same directory as the `.fsproj` file.

## F# projects (.fsproj)

Fable can compile `.fsx` scripts. But when your project becomes a bit complicated, it's recommended to
use a project file (`.fsproj`). F# project files are in XML format, follow MSBuild specifications and are
not very human readable (though this will improve with MSBuild 15) so it's recommended to use an IDE like Visual
Studio or [Ionide](http://ionide.io/) to handle them.

> Remember that files listed in `.fsproj` must be in sorted compilation order.

To avoid the MSBuild dependency, at the moment Fable uses [Forge](http://forge.run/) internally to parse
`.fsproj` files. However, this kind of parsing is not as powerful as using MSBuild itself so it's a good
idea to keep your project files simple. Only source files and `.dll` references are read from the `.fsproj`
file (use the `--symbols` Fable compiler argument if you want to define compilation constants like `DEBUG`),
and conditional MSBuild items are not supported.

> By default, Fable always define the `FABLE_COMPILER` compilation constant.

## Project references

Above it's mentioned that Fable can read `.dll` references from `.fsproj` files. However, project references
cannot be read directly. Instead you must pass all the projects you want to compile at once to the `--projFile`
argument.

```shell
node node_modules/fable-compiler src/MyLib/MyLib.fsproj src/MyApp/MyApp.fsproj
```

Fable will actually merge the files in all projects for the build, so it's important that project
files are also listed in **compilation order**.

> In the `.fsproj` file you still need to use project references so IDEs like Visual Studio or Ionide work properly.

## fableconfig.json

Rather than passing all the options to the CLI, it may be more convenient to put them in JSON format
([JSON5](http://json5.org/) is also supported) in a file named `fableconfig.json` and let the compiler read them for you.
You can combine options from the CLI and `fableconfig.json`, when this happens the former will have preference.

To use `fableconfig.json` just pass the directory where the JSON file resides to Fable.
If you omit the path, the compiler will assume it's in the current directory.

```shell
fable my/path/
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


## fable-core

[Fable's core library](https://github.com/fable-compiler/Fable/blob/master/import/core/fable-core.js) must be included in the project.
When targeting node or using a module bundler you only need to add the dependency:

```shell
npm install --save fable-core
```

If targeting the browser and using AMD instead (`--module amd`), you can load Fable's core lib with
[require.js](http://requirejs.org) as follows:

```html
<script src="node_modules/requirejs/require.js"></script>
<script>
requirejs.config({
    // Set the baseUrl to the path of the compiled JS code
    baseUrl: 'out',
    paths: {
        // Explicit path to core lib (relative to baseUrl, omit .js)
        'fable-core/umd': '../node_modules/fable-core/umd'
    }
});
// Load the entry file of the app (use array, omit .js)
requirejs(["app"]);
</script>
```

> Note that when targeting a UMD-compatible module (like commonjs or amd), Fable will automatically
pick the `fable-core/umd` distribution.

## Polyfill

By default Fable will compile to ES5 syntax (you can change that with `--ecma es2015` argument),
but it still uses some classes that belong to ES2015 specification (like `Symbol`). So if you want to
run the code in old browsers, you need to poliyfill them. There are two ways to do this:

- Load [core-js](https://github.com/zloirock/core-js) with a script tag before loading Fable's code:

```html
<script src="node_modules/core-js/client/shim.min.js"></script>
```

- Use [babel-runtime](https://babeljs.io/docs/plugins/transform-runtime/) plugin. This will add
  only the polyfills your code needs automatically.

```shell
npm install --save babel-runtime
npm install --save-dev babel-plugin-transform-runtime
```

```json
// In fableconfig.json
"babelPlugins": ["transform-runtime"],
```

## Modules

The compiler will keep the file structure of the F# project, converting each file to an [ES2015 module](https://github.com/lukehoban/es6features#modules).
When the `--module` argument is passed to the compiler, these modules will be transformed in the JS code to
[umd](https://github.com/umdjs/umd), [amd](http://requirejs.org/docs/whyamd.html), [commonjs](https://nodejs.org/docs/latest/api/modules.html).
If `--module` is omitted, ES2015 `import/export` modules will be kept in the final code.

When an F# file makes a reference to another, the compiler will automatically create an [import statement](https://developer.mozilla.org/en/docs/web/javascript/reference/statements/import)
in the generated Javascript code. You can also generate imports by yourself, see [Interacting with JS](interacting.html).

If the F# file just contains a single root module (the namespace doesn't matter) Fable will expose its public
members with the ES2015 [export](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Statements/export) keyword.
This has many benefits for [tree shaking](http://fable.io/blog/Tree-shaking.html) (see below) so for Fable projects
it's recommended to have just one module per file and avoid nested modules when possible.

## Bundling

Very often JS apps are bundled, that is, the main script and all its dependencies
are brought together into a single file to make deployment much easier. Bundlers have
also other benefits, as they can optimize code, remove unused parts and minify the result.

Fable comes with [Rollup](http://rollupjs.org/) embedded, a bundler for ES2015 modules
(the default since Fable 0.7.x) with [tree shaking](http://fable.io/blog/Tree-shaking.html) capabilities.
To activate you only need to pass the `--rollup` flag.

In `fableconfig.json` it's also possible to pass a configuration object to the `rollup` option.
Check [Rollup JS API](https://github.com/rollup/rollup/wiki/JavaScript-API) for the available options.
However, please note that plugins use the same format as Babel plugins ([reference](https://babeljs.io/docs/plugins/#plugin-preset-options)):

```json
"rollup": {
  "plugins": [
    ["commonjs", {
      "namedExports": {
        "virtual-dom": [ "h", "create", "diff", "patch" ]
      }
    }]
  ]
}
```

The default Rollup configuration in Fable is:

```js
{
    // entry: <Last F# source file>
    dest: fableOptions.outDir + "/bundle.js",
    format: fableOptions.module || "iife",
    sourceMap: fableOptions.sourceMaps,
    moduleName: normalizeProjectName(fableOptions),
    plugins: {
        require('rollup-plugin-node-resolve')({ ignoreGlobal: true }),
        require('rollup-plugin-commonjs')({ jsnext: true, main: true, browser: true })
    }
}
```

If you prefer to use other bundlers like [Webpack](https://webpack.js.org/) you just need
to call it after Fable compilation. For example, in the `postbuild` script (or `postbuild-once`
if using watch mode).

> **Caveat**: Though Rollup can understand commonjs modules, it's not as powerful as Webpack
and can have problems bundling some packages (like React). In this case you may want to
load the JS library globally with a `<script>` tag ([example](https://github.com/fable-compiler/Fable/blob/bf0192366c22929c6b589b5b39bec52512d6d7df/samples/browser/redux-todomvc/fableconfig.json)).

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