// Opens a URL from the browser's own click event. Unity handles a tap a frame
// after the click, which pop-up blockers can treat as unrequested, so
// OpenURLButton arms this on pointer down and the page opens the tab when the
// click arrives. If frames are slow and the click already passed, Unity's click
// opens it directly (still inside the browser's user-activation window), and an
// armed link expires so it can never fire on a later, unrelated click.
mergeInto(LibraryManager.library, {
  OpenUrlOnNextClick: function (urlPointer) {
    var url = UTF8ToString(urlPointer);
    var pending = Module.unityTemplatePendingLink;
    if (pending) document.removeEventListener('click', pending.open, true);
    pending = { url: url, armedAt: Date.now() };
    pending.open = function () {
      document.removeEventListener('click', pending.open, true);
      if (Module.unityTemplatePendingLink === pending) Module.unityTemplatePendingLink = null;
      if (Date.now() - pending.armedAt < 5000) window.open(pending.url, '_blank', 'noopener');
    };
    Module.unityTemplatePendingLink = pending;
    document.addEventListener('click', pending.open, true);
  },

  OpenUrlIfStillPending: function () {
    var pending = Module.unityTemplatePendingLink;
    if (pending) pending.open();
  },

  CancelOpenUrlOnNextClick: function () {
    var pending = Module.unityTemplatePendingLink;
    if (!pending) return;
    document.removeEventListener('click', pending.open, true);
    Module.unityTemplatePendingLink = null;
  }
});
