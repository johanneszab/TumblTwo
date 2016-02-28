# TumblTwo
TumblTwo, an Improved Fork of TumblOne, a Tumblr Image Downloader.
___
TumblTwo, a TumblOne Fork:
TumblTwo is an image downloader for the Bloghoster Tumblr.com largely
based on TumblOne by Helena Craven. After supplying a url, the tool 
will search and download all types of images in a given resolution.
You can simultaneously download from multiple blogs and enqueue others. 

## New Features (over TumblOne):
* multiple simultaneous picture downloads of a single blog, customizable in the settings. As an alternative, each picture is downloaded successively.
* multiple simultaneous downloads of different blogs, customizable in the settings.
* it is possible to download images from blogs only for specific tags.
* a clipboard monitor that detects *http:// .tumblr.com* urls in the clipboard (copy and paste) and automatically adds the blog to the bloglist.
* a download queue for blogs.
* a detection if the blog is still online or the owner has changed.
* the blogview is now sortable and shows more information, e.g. date added, last time finished and the progress.
* a settings panel (change download location, turn picture preview off/on, define number of simultaneous downloads, set the imagesize of downloaded pictures).
* Somewhat overhauled user interface which is resizable, faster and saves and restores its settings.
* Source code at github (Written in C# and WinForms).

## Next Features (ToDo-List):

* Complete code rewrite to remove the spaghetti-code and migrate to WFP (MVVM) instead of WinForms.

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
