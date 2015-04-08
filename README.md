# TumblTwo
TumblTwo, an Improved Fork of TumblOne, a Tumblr Image Crawler.
<br><br>
TumblTwo, a TumblOne Fork:
TumblTwo is an image crawler for the Bloghoster Tumblr.com largely
based on TumblOne by Helena Craven. After supplying a url, the tool 
will search and download all types of images in a given resolution.
You can simultaneously download from multiple blogs and enqueue others. 

New Features
New Features (over TumblOne):

    2015-04-08:
        multiple simultaneous downloads
        a download queue
        a settings panel (change download location, turn picture preview off/on, define number of simultaneous downloads, set imagesize of downloaded pictures)
        the tumblrlist now features columns for 'Date added' and if and when the blog was completely crawled
        saves and restores settings


Next Features (ToDo-List):

    prevent downloading "Image has been removed" / same images
    option to automatically remove blogs when crawling is complete
    batch input of tumblr blog urls from text file


Bugs:

    fix the heavy UI lag during crawling (likely to many 'base.invokes' from the background tasks to the main update ui thread. There is currently one after every downloaded image.)

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
