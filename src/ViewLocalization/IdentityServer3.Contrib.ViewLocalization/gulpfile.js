/// <binding BeforeBuild='build' />
var gulp = require("gulp"),
    header = require("gulp-header"),
    del = require("del"),
    concat = require("gulp-concat"),
    uglify = require("gulp-uglify"),
    sourcemaps = require("gulp-sourcemaps"),
    minifyCss = require("gulp-minify-css"),
    less = require('gulp-less'),
    pkg = require("./package.json"),
    Config = require("./gulpfile.config");

var config = new Config();

var banner = ['/**',
  " * <%= pkg.name %> - <%= pkg.description %>",
  " * @version v<%= pkg.version %> - <%= new Date().toISOString() %>",
  " * @license <%= pkg.license %> - <%= new Date().getFullYear() %>",
  " */",
  ""].join('\n');

gulp.task("copy-fonts", function () {
    return gulp.src(config.fontFiles)
        .pipe(gulp.dest("./wwwroot/fonts/"));
});

gulp.task("copy-pages", function () {
    return gulp.src(config.allAppHTML)
        .pipe(gulp.dest("./wwwroot/"));
});

gulp.task("all-css", ["app-less-compile"], function () {

    config.vendorCSSFiles.push(config.allAppCss);

    return gulp.src(config.vendorCSSFiles)
        .pipe(sourcemaps.init())
        .pipe(concat(config.mainCssFile))
        .pipe(minifyCss())
        .pipe(header(banner, { pkg: pkg }))
        .pipe(sourcemaps.write('.', { sourceRoot: './' }))
        .pipe(gulp.dest(config.cssOutputPath));
});

gulp.task("app-less-compile", function () {
    return gulp.src(config.allAppLess)
         .pipe(sourcemaps.init())
         .pipe(less())
         .pipe(sourcemaps.write('.'))
         .pipe(gulp.dest(config.sourceApp));;
});

gulp.task("libs-css", function () {
    return gulp.src(config.vendorCSSFiles)
        .pipe(sourcemaps.init())
        .pipe(concat("libs.min.css"))
        .pipe(minifyCss())
        .pipe(header(banner, { pkg: pkg }))
        .pipe(sourcemaps.write('.', { sourceRoot: './' }))
        .pipe(gulp.dest(config.cssOutputPath));
});

gulp.task("all-js", ["libs-js", "app-js"], function () {

    var temps = [
        config.jsOutputPath + '/libs.min.js',
        config.jsOutputPath + '/app.min.js'
    ];

    return gulp.src(temps)
               //.pipe(sourcemaps.init({ loadMaps: true }))
               .pipe(concat(config.mainJsFile))
               .pipe(header(banner, { pkg: pkg }))
               //.pipe(sourcemaps.write(".", { sourceRoot: "./" })) // You can use other plugins that also support gulp-sourcemaps
               .pipe(gulp.dest(config.jsOutputPath));
});

gulp.task("libs-js", function () {

    return gulp.src(config.vendorJSFiles)
        //.pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(concat("libs.min.js"))
        //.pipe(sourcemaps.write(".", { sourceRoot: "./" }))
        .pipe(gulp.dest(config.jsOutputPath));

});

gulp.task("app-js", function () {

    return gulp.src(config.allAppJS)
        //.pipe(sourcemaps.init())
        .pipe(concat("app.min.js")) // You can use other plugins that also support gulp-sourcemaps
        .pipe(uglify())
        //.pipe(sourcemaps.write(".", { sourceRoot: "./" })) // Now the sourcemaps are added to the .js file
        .pipe(gulp.dest(config.jsOutputPath));
});

gulp.task("clean-temp", ["all-js"], function (cb) {
    var temps = [
        config.jsOutputPath + '/libs.min.js*',
        config.jsOutputPath + '/app.min.js*'
    ];

    // delete the files
    del(temps, cb);
});

gulp.task("clean-root", function (cb) {
    var cleanFiles = [
        "wwwroot/content/**/*", "!wwwroot/content/site.css", "wwwroot/scripts/**"
    ];

    // delete the files
    del(cleanFiles, cb);
});

gulp.task("default", ["copy-fonts", "copy-pages", "app-less-compile", "all-css", "libs-js", "app-js", "all-js", "clean-temp"]);

gulp.task("build", ["copy-pages", "app-less-compile", "all-css", "libs-js", "app-js", "all-js", "clean-temp"]);