/// <binding BeforeBuild='clean:dist, html' />
var gulp = require('gulp');
var useref = require('gulp-useref');
var uglify = require('gulp-uglify');
var filter = require('gulp-filter');
var cssmin = require('gulp-csso');
var imagemin = require('gulp-imagemin');
var cache = require('gulp-cache');
var del = require('del');
var runSequence = require('run-sequence');
var rev = require('gulp-rev');
var revReplace = require('gulp-rev-replace');
var htmlmin = require('gulp-htmlmin');
//var uncss = require('gulp-uncss');
var pump = require('pump');
var inline = require('gulp-inline-source');
var rename = require('gulp-rename');
var replace = require('gulp-replace');
var sourcemaps = require('gulp-sourcemaps');

var siphon = require('siphon-media-query');
var inky = require('inky');
var sass = require('gulp-sass');
var inlineCss = require('gulp-inline-css');
var lazypipe = require('lazypipe');
var fs = require('fs');
var babel = require("gulp-babel");

//var foundationEmailCss = 'wwwroot/lib/foundation-emails/_build/assets/css';


gulp.task('html', function(cb){
    var mainFile = './index.html';
    //var glob primer - https://www.npmjs.com/package/glob
    var jsFilter = filter("**/*.js", { restore: true });
    var cssFilter = filter("**/*.css", { restore: true });
    var fontFoldersToMove = ["./wwwroot/lib/*/fonts/*.*",""];
    var fontFilesToMove = ["./wwwroot/lib/*/*.{eot,svg,ttf,woff,woff2}", "./wwwroot/lib/videogular-themes-default/videogular.min.css"];
    var imageFilesToMove = ["./wwwroot/lib/world-flags-sprite/images/*.png"];
    var debugFilename = 'index_debug';

    /* pump([gulp.src(mainFile),
        rename({ basename: debugFilename }),
        replace('<base href="/">', '<base href="/'+debugFilename+'.html">'),
        inline({ compress: false }),
        //useref({noconcat:true}),
        gulp.dest('dist')]); */

    pump([gulp.src(mainFile),
        replace(/<!--debug-->(.|[\r\n])*<!--enddebug-->/, ''),
        inline({ compress: true }),
        useref({}, lazypipe().pipe(sourcemaps.init, { loadMaps: true })),
        //uncss({ html: [mainFile, 'app/**/*.html'] }), //needs to have access to css, jss and html
        jsFilter,
        babel(),
        uglify(), //.on('error', console.log), //if unable to uglify, note the line number of the error, comment out this line, gulp html & then view new .js file (in dist/js)
        rev(),
        jsFilter.restore,
        cssFilter,
        cssmin(),
        rev(),
        cssFilter.restore,
        revReplace(/*{modifyReved: replaceJsIfMap}*/),
        sourcemaps.write('maps'),
        gulp.dest('dist') 
    ], cb);

    gulp.src(fontFoldersToMove)
        .pipe(rename({ dirname: '' }))
        .pipe(gulp.dest('dist/fonts'));

    gulp.src(fontFilesToMove)
        .pipe(rename({ dirname: '' }))
        .pipe(replace(/\bfonts\/videogular\./g, '../fonts/videogular.'))
        .pipe(gulp.dest('dist/css'));

    gulp.src(imageFilesToMove)
    .pipe(rename({ dirname: '' }))
    .pipe(gulp.dest('dist/images'));

    function replaceJsIfMap() {
        console.log(JSON.stringify(arguments));
        return arguments[0];
    }
});

// Optimizing Images 
gulp.task('images', function () {
    return gulp.src('app/images/**/*.+(png|jpg|jpeg|gif|svg)')
      // Caching images that ran through imagemin
      .pipe(cache(imagemin({
          interlaced: true
      })))
      .pipe(gulp.dest('dist/images'));
});

gulp.task('videos', function () {
    return gulp.src('videos/*.+(mp4|ogg|webm)')
      // Caching images that ran through imagemin
      //.pipe(replace(/<BaseURL>(.*)\.mp4<\/BaseURL>/, '<BaseURL>api/videos/dash/$1</BaseURL>'))
      .pipe(gulp.dest('dist/videos'));
});

// Copying fonts 
gulp.task('fonts', function () {
    return gulp.src('app/fonts/**/*')
      .pipe(gulp.dest('dist/fonts'));
});

// Cleaning 

gulp.task('clean:dist', function () {
    return del.sync(['dist/index*.html','dist/**/*.js', 'dist/**/*.css', '!dist/images', '!dist/images/**/*']);
});

gulp.task('build', function (callback) {
    runSequence(
      'clean:dist',
      ['html', 'images', 'fonts'],
      callback
  );
});

gulp.task('parseInky', function () {
    var fn = getArgs(process.argv, "--file");
    gulp.src(fn
        ? 'UserEmails/Inky/' + fn
        :'UserEmails/Inky/**/*.cshtml')
        .pipe(inky()) //NOT WORKING { cheerio: { lowerCaseTags: false } }
        .pipe(inliner('wwwroot/lib/foundation-emails/dist/foundation-emails.css'))
        .pipe(gulp.dest('UserEmails'));
});

function getArgs(argv, optionName) {
    var i = argv.indexOf(optionName);
    if (i > -1) {
        return argv[i + 1];
    }
    return null;
}

// Compile Sass into CSS
// Compiles Foundation-specific CSS
/*
gulp.task('sass:foundation', function () {
    return gulp.src('wwwroot/lib/foundation-emails/scss/foundation-emails.scss')
      .pipe(sass().on('error', sass.logError))
      .pipe(gulp.dest('wwwroot/lib/foundation-emails/_build/assets/css'));
});
*/

function inliner(css) {
    var cssString = fs.readFileSync(css).toString();
    var mqCss = siphon(cssString).replace(/@/g, "@@");
    var pipe = lazypipe()
      .pipe(inlineCss, {
          extraCss: cssString,
          applyStyleTags: true,
          removeStyleTags: true,
          removeLinkTags: true
      })
      .pipe(replace, '<!-- <style> -->', '<style>' + mqCss + '</style>');
        /*
      .pipe(htmlmin, {
          collapseWhitespace: true,
          minifyCSS: true,
          conservativeCollapse: true,
          decodeEntities: true,
          html5: false,
          quoteCharacter: "'",
          ignoreCustomFragments: [/^@\*[\s\S]+</] 
      })
    //.pipe(replace, '@using ', '\r\n@using ')
    //.pipe(replace, '@inherits ', '\r\n@inherits ')
    //.pipe(replace, '@functions', '\r\n@functions');
    */
    return pipe();
}
