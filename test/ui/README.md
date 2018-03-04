# Unit Testing

A simple setup has been provided using mocha-webpack and jsdom. Use

`npm run watch-test`

Some points to note are

## Webpack

Webpack is set up to resolve imported dependencies from _test/ui/node_modules_. So if you are testing a component from _src/ui_ (and that component in turn imports a non-relative module) you will need to

* add the same module to the package.json file in _test/ui_ __or__
* add an _alias_ in webpack.config.js that resolves to _src/ui/node_modules/&lt;alias&gt;_ __&amp;__ then _whitelist the alias_ so it is included in the actual bundle

## Events

When writing unit tests to simulate user interaction with the browser, you will be modify DOM elements of the component via code. After making changes  you will often need to _manually trigger HTML events_.

This will trigger Vue to make an async update to the underlying virtual DOM.

After this update you can then make assertions by accessing [vm.$el](https://vuejs.org/v2/api/#vm-el). See the official documentation on [vm.$nextTick([callback])](https://vuejs.org/v2/api/#vm-nextTick) for details