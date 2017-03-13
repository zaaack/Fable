var path = require('path');
var fs = require('fs');
var child_process = require("child_process");

function resolve(filePath) {
  return path.resolve(__dirname, filePath)
}

var babelOptions = {
  "presets": [
    [resolve("./node_modules/babel-preset-es2015"), {"modules": false}]
  ]
}

module.exports = {
  entry: resolve('./ts2fable.fsproj'),
  output: {
    filename: 'ts2fable.js',
    path: resolve('npm/'),
  },
  target: "node",
  module: {
    rules: [
      {
        test: /\.fs(x|proj)?$/,
        use: {
          loader: "fable-loader",
          options: { babel: babelOptions }
        }
      },
      {
        test: /\.js$/,
        exclude: /node_modules\/(?!fable)/,
        use: {
          loader: 'babel-loader',
          options: babelOptions
        },
      }
    ]
  }
};
