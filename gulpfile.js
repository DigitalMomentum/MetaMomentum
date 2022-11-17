/// <binding ProjectOpened='copyfiles' />
var gulp = require('gulp');
var watch = require('gulp-watch');

gulp.task('copyfiles', function (cb) {
	var devFiles = ['MetaMomentum/App_Plugins/**/*', 'src/js/**/*.js'];

	//gulp.watch('.MetaMomentum/App_Plugins/**/*', gulp.series("saas"));
	//gulp.watch(devFiles, function (files) {
	//	console.log("watch");
	//		}
	//);

	//gulp.src(devFiles, { base: "./" })
	//	.pipe(watch(devFiles, { base: "./" }))
	//	.pipe(gulp.dest("./"))


	var source = '././MetaMomentum/App_Plugins',
		destination = ['./MetaMomentum.UmbracoV10/App_Plugins', './MetaMomentum.UmbracoV9/App_Plugins', './MetaMomentum.UmbracoV8/App_Plugins'];


	gulp.src(source + '/**/*', { base: source })
		.pipe(watch(source, { base: source }))
		.pipe(gulp.dest('./MetaMomentum.UmbracoV8/App_Plugins'))
		.pipe(gulp.dest('./MetaMomentum.UmbracoV9/App_Plugins'))
		.pipe(gulp.dest('./MetaMomentum.UmbracoV10/App_Plugins'));



	cb()
});

