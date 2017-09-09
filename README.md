# TumblTwo

TumblTwo, an Improved Fork of TumblOne, a Tumblr Image Downloader.

**New**: Check out [TumblThree](https://github.com/johanneszab/TumblThree) for my beginning code rewrite using C# with WPF and the MVVM pattern.

___

TumblTwo, a TumblOne Fork:
TumblTwo is an image downloader for the Bloghoster Tumblr.com largely based on TumblOne by Helena Craven. After supplying a url, the tool will search and download all types of images in a given resolution. You can simultaneously download from multiple blogs and enqueue others. 

## New Features (over TumblOne):

* multiple simultaneous picture downloads of a single blog, customizable in the settings. As an alternative, each picture is downloaded successively.
* multiple simultaneous downloads of different blogs, customizable in the settings.
* Download of tumblr.com hosted videos
* it is possible to download images from blogs only for specific tags.
* a clipboard monitor that detects *http(s):// .tumblr.com* urls in the clipboard (copy and paste) and automatically adds the blog to the bloglist.
* a download queue for blogs.
* a detection if the blog is still online or the owner has changed.
* the blogview is now sortable and shows more information, e.g. date added, last time finished and the progress.
* a settings panel (change download location, turn picture preview off/on, define number of simultaneous downloads, set the imagesize of downloaded pictures).
* Somewhat overhauled user interface which is resizable, faster and saves and restores its settings.
* Source code at github (Written in C# and WinForms).

## Screenshot:

![TumblTwo Main UI](http://www.jzab.de/sites/default/files/images/TumblrTwoUi201603.png?raw=true "TumblTwo Main UI")

## Application Usage:

  * To use the application, simply copy the url of any tumblr.com blog you want to download the pictures from into the textbox at the top. Afterwards, click on 'Add Blog' on the right.
  * To start the crawl process, click on 'Crawl' on the right. The application will regularly check for (new) blogs in the queue and start processing them, until you stop the application by pressing 'Stop'. So, you can either add blogs to the queue via 'Add to Queue' first and then click 'Crawl', or you start the crawl process first and add blogs to the queue afterwards.
  * You can set up more than one parallel download in the 'Settings' on the right side. Also, it is possible to change the download location and the sizes of the pictures to download there.

### Tags:

  * You can also download only tagged images by adding tags in a comma separated list in the tag column of the blog list in the top. For example: *great big car*, *bears* would search for images that are tagged for either a *great big car* or *bears* or both.

### Performance:

  * If the download stalls after a period of time and just finishes incompletely, you might have to lower the Number of parallel image downloads for all blogs in the settings panel. Most likely the application has opened too many connections to the tumblr network which were timed out and got closed by the servers. Try to recrawl with lower values. The applications restarts where it left off.
  * Otherwise, if the download speeds are not satisfied, you may increase the value.
  * If the download still behaves weird, unticking crawl blogs in parallel in the settings will increase stability on the cost of speed. Images are then download one by one.

## Possible Next Features (ToDo-List):

* Complete code rewrite to remove the spaghetti-code and migrate to WFP (MVVM) instead of WinForms.

I'm completely new to C# and (safe)-threading programming and if anyone 
wants to help, feel free to commit. So, beware of the code ;). I'll add
source code annotations over the next few days and the first git commit is
the pure reverse engineered TumblOne code without any modifications from my
side.

## License:

Since I had to reverse engineer TumblOne (simple reflecting) and TumblOne
is under the public domain (http://sourceforge.net/projects/tumblone/), I
decided to release the source code with my changes under the public domain
as well.
