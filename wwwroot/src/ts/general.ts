// import 'jquery';

export function fixHeight() {
  const cardTextArea = $(this);
  cardTextArea.height(0);
  cardTextArea.height(cardTextArea.prop('scrollHeight'));
}

export function removeParamFromURL(param, reloadAfter = true) {
  const parsedUrl = new URL(window.location.href);
  parsedUrl.searchParams.delete(param);
  const changedUrl = parsedUrl.toString();

  if (reloadAfter) {
    window.location.href = changedUrl;
  }
  else {
    return changedUrl;
  }
}

export function addParamToURL(param, value, reloadAfter = true) {
  const parsedUrl = new URL(window.location.href);
  parsedUrl.searchParams.set(param, value);
  const changedUrl = parsedUrl.toString();

  if (reloadAfter) {
    window.location.href = changedUrl;
  }
  else {
    return changedUrl;
  }
}