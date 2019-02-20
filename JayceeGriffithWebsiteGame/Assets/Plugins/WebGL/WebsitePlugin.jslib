mergeInto(LibraryManager.library, {
  SetPage: function(page) {
    ReactUnityWebGL.SetPage(Pointer_stringify(page));
  },
  OnReady: function() {
    ReactUnityWebGL.OnReady();
  },
  MouseEvent: function(mouseEvent) {
    ReactUnityWebGL.MouseEvent(Pointer_stringify(mouseEvent));
  },
  GetDimensions: function() {
	ReactUnityWebGL.GetDimensions();
  }
});