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

gulp.task('html', function(cb){
    var mainFile = './index.html';
    //var glob primer - https://www.npmjs.com/package/glob
    var jsFilter = filter("**/*.js", { restore: true });
    var cssFilter = filter("**/*.css", { restore: true });
    var fontFoldersToMove = ["./wwwroot/lib/*/fonts/*.*"];
    var fontFilesToMove = ["./wwwroot/lib/*/*.{eot,svg,ttf,woff,woff2}"];
    var debugFilename = 'index_debug';

    pump([gulp.src(mainFile),
        rename({ basename: debugFilename }),
        replace('<base href="/">', '<base href="/'+debugFilename+'.html">'),
        inline({ compress: false }),
        gulp.dest('dist')]);

    pump([gulp.src(mainFile),
        inline({ compress: true }),
        useref(),
        //uncss({ html: [mainFile, 'app/**/*.html'] }), //needs to have access to css, jss and html
        jsFilter ,
        uglify(),
        rev(),
        jsFilter.restore,
        cssFilter,
        cssmin(),
        rev(),
        cssFilter.restore,
        revReplace(/*{modifyReved: replaceJsIfMap}*/),
        gulp.dest('dist') 
    ], cb);

    gulp.src(fontFoldersToMove)
        .pipe(rename({ dirname: '' }))
        .pipe(gulp.dest('dist/fonts'));

    gulp.src(fontFilesToMove)
        .pipe(rename({ dirname: '' }))
        .pipe(gulp.dest('dist/css'));

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

// Copying fonts 
gulp.task('fonts', function () {
    return gulp.src('app/fonts/**/*')
      .pipe(gulp.dest('dist/fonts'));
});

// Cleaning 

gulp.task('clean:dist', function () {
    return del.sync(['dist/**/*', '!dist/images', '!dist/images/**/*']);
});

gulp.task('build', function (callback) {
    runSequence(
      'clean:dist',
      ['html', 'images', 'fonts'],
      callback
  );
});