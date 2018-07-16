# Toucan - UI

## Bootstrap

Only a small amount of customization has been done to the default bootstrap theme. Both scripts and styles are referenced from local /node_modules folder during the build process.

For the predefined dev & production builds, webpack is configured to extract css into external files

## Webpack

There are multiple areas defined in the client Vue.js application ... 

* ./ui/app/root/root.ts => https://localhost:5001
* ./ui/app/admin/admin.ts  => https://localhost:5001/admin

The motive behind this decision was to

* optimize end user experience (by ensuring only required resources are downloaded)
* enable switching between layouts in different areas of a site
* establish a common set of components for re-use

To support multiple entry points as described above, the webpack build process is configured to emit multiple files

* vendor.js - contains core third-party dependencies
* common.js - contains code used by all areas/subsites
* common.css - contains code used by all areas/subsites
* [area].js - contains area-specific code and resources
* [area].css - contains area-specific code and resources

## Html

Default html files for each area are automatically injected with links to any asset bundles produced by webpack.

## Supported Browsers

TypeScript source files are transpiled by babel-loader, and try to keep it fairly fresh - _target last two versions_ is the default specified in babel.rc configuration

Polyfills are also provided by the babel loader if native browser support is inadequate

## Project Structure

* _assets_ - static resources for inclusion in client bundles
* _common_ - utility classes
* _components_ - store global components in this folder
* _locales_ - language resource files
* _model_ - models used by multiple areas
* _routes_ - vue.js router navigation guards
* _services_ - helper/domain classes
* _store_ - vuex store used by global components
* _styles_ - global SASS styles
* _validation_ - shared validations rules

Other components will be found in specific sub-areas (such as 'root' or 'admin'). These areas have a similar structure to the project root, but at minimum will contain

* _layout_  - contains any custom layouts
* _navigation_  - navbar or menu components for use by layouts
* _routes_  - route configuration, and route names
* _store_ - a custom vuex store that extends the default vuex store
* _[area].ts_ - code to bootstrap the entry point
* _[area].html_ - entry point page
* _[area].scss_ - area SASS styles