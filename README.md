# TumblTwo
TumblTwo, an Improved Fork of TumblOne, a Tumblr Image Crawler.
<br><br>
TumblTwo, a TumblOne Fork:
TumblTwo is an image crawler for the Bloghoster Tumblr.com largely
based on TumblOne by Helena Craven. After supplying a url, the tool 
will search and download all types of images in a given resolution.
You can simultaneously download from multiple blogs and enqueue others. 

New Features (over TumblOne):

	2016-02-19: stable

		It's now possible to download photosets
		Added a detection if the blog is still alive and/or if its the same blog. Therefore we use the HTML Title and the blog description. I wasn't sure if the title would be enough, since many blog titles are simply equal to the url, which might not change if the owner the blog changes. Thus, I'm also taking the description into account, but I'm not sure if they frequently change. So, I'm happy about any input in the comments/per mail about this if we're generating too many false positives.

	2015-11-23: all releases

		Allows Column Sorting
		Added a process percentage column in the Blogview (no fancy progressbars yet).
		"Delete Blog" now deletes only the index file and removes the blog from the view, but does not delete any downloaded images.
		"Remove completely crawled blogs" now does something and removes the blog (and index) after a finished crawl, but keeps the images.

	2015-09-08: all releases

		Fix for the "Current Blog cannot be saved to Disk!"-Bug that happens when you changed the default download location to anything else than ending with \blogs\. This time, I hope for real. That's the pay for spaghetti-code. I overlooked a ".\Blogs\."-path in the old TumblOne Code. Special thanks to Jerome and James for keep pointing this out!
		It's now possible to import TumblOne-Blogs by simply addind/moving the proper .tumblr files from the Index folder of TumblOne (which is also located inside the \Blogs\ folder which holds your downloaded pictures right next to where the TumblOne.exe is located) into the Index folder of your download location set in the 'Settings' window in TumblTwo. The blogs will be added but will be showing a "not yet crawled!". Thats okay, because we use a different counting mechanism. After starting the first crawl, the proper index will be adjusted.
		For an update on video / larger image support, see here

	2015-09-01: stable and beta release

		Removing a blog is now always possible and does not result in a reload of the whole library (not sure, why this was implemented in the first way.)
		Some fixes for the progressbar.
		Some minor UI code changes and cleanup.

	2015-08-28: beta release

		Fixed a completely non-working release. Might work now..

	2015-08-28: stable and beta release

		Added a Clipboard Monitor. Enabled by default, can be turned off in the mainwindow on the right side panel. Once turned on, if you ctrl-c or copy any text which contains one or more Tumblr blog urls, the blogs will be automatically added if they don't exist.
		Disabled useless startup splashscreen.

	2015-08-27:

		Beta release (Not really well tested yet). Crawl only specifically tagged images. Crawl only specifically tagged images by specifying the tags in the Queue Window in a comma separated way. I.e: Aston Martin,ferrari,Porsche. Consequently, the Blog is crawled for any image that matches the given tags. To do so: add the desired blog to the queue, without starting the crawl process. Now click in the cell next to the blog with the column header Tags for crawling. Enter your tags in a comma separated way, finish with enter. Start the crawl. If you don't bother about tags, simply don't add anything to crawl the whole blog.

	2015-08-26:

		Fixed threading wonkiness. Sometimes, the queue still got depleted from idling tasks after pressing 'stop'.
		Adjust the number of simultaneous downloads without a necessary restart of the application, if the number of threads is not smaller than it was before and if the crawl process is not currently running.
		Make sure the download location is always correct to fix the "The current blog cannot be saved to disk"-bug.
		Now saving the windowsize and its position.
		New (more human) color scheme:

	2015-08-25:

		Large Speedup for startup times and resuming of blogs since we now catalogize all downloaded filenames together with their URLs in a small single index file, instead of checking for all single downloaded image files in the download folder at startup and which is now also used for duplication check. This should improve speed drastically. Also, its now possible to safely remove images out of the \Blogs\"MyDownloadedTumblrBlogFolder"\ without rendering in download them again, as long as you keep the .tumblr (index) file in the Index folder. This opens the way to a backup function.
		Specifying the number of Posts in each blogs. Might be equivalent to number of pictures, if the blog only contains pictures.

	2015-08-24:

		Fixed the UI stall on resuming large blogs. This time for real. There is simply a message now and no progress indicator anymore since it causes the Interface to stall because its too fast to display. You still can open the task manager and look at the Network Traffic panel. The application compares the files in the blogs with the already downloaded files and skips them.
		Changing the download location now reloads the settings and the library (if available) after closing the settings window.
		Better indicator after finishing downloading the last blog from the queue.

	2015-08-21:

		fixed broken queue function (sorry, didn't test it enough after the last changes).
		Speedup for resuming on large blogs.

	2015-06-06:

		add and remove multiple blogs at once to queue (multiple selection)

	2015-06-04:

		Added Multiselection in the Blog and Queue View.
		To add multiple blogs at once to the queue, select the blogs with the ctrl-key or shift-key pressed, then hit "Add to Queue". Same for removing, just in the "Queue"-view and hit "Remove Queue" (Thanks to Torn for suggesting this!).

	2015-06-01:

		Fixed: The FolderBrowserDialog returned a path without trailing backslash which resulted in a variety of different errors. (Thanks to H3 pointing this out!). See the comments for more.

    2015-04-08:
        multiple simultaneous downloads
        a download queue
        a settings panel (change download location, turn picture preview off/on, define number of simultaneous downloads, set imagesize of downloaded pictures)
        the tumblrlist now features columns for 'Date added' and if and when the blog was completely crawled
        saves and restores settings


Next Features (ToDo-List):

    Complete code rewrite to remove the spaghetti-code and migrate to WFP (MVVM) instead of WinForms.

I'm completely new to C# and (safe)-threading programming and if anyone 
wants to help, feel free to commit. So, beware of the code ;). I'll add
source code annotations over the next few days and the first git commit is
the pure reverse engineered TumblOne code without any modifications from my
side.

Current Binaries can also be found at: http://www.jzab.de/content/tumbltwo

Since I had to reverse engineer TumblOne (simple reflecting) and TumblOne
is under the public domain (http://sourceforge.net/projects/tumblone/), I
decided to release the source code with my changes under the public domain
as well.
