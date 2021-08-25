import { toggleFilterTagState } from "./index";
import * as bootbox from 'bootbox';
import { fixHeight } from "../general";
import feather from 'feather-icons';

import '@popperjs/core';
import tippy from 'tippy.js';
// import $ from "jquery";

const copypastas = $(".copypasta");

function createTag(tagId) {
  return $("<span/>", {
      "data-href": `/tagId=${tagId}`,
      "class": "tag",
      "data-tag-id": tagId,
      "text": all_tags.find(el => el.id === tagId).name,
  });
}

function createRemoveTagButton() {
  return $("<button/>", {
      "class": "tag__remove-button"
  }).append(
      $("<i/>", {
          "class": "bi bi-x-circle"
      })
  );
}

function bootboxVisualFix() {
  // Fix visual issue
  const closeButton = $(".bootbox-close-button");
  closeButton.toggleClass("close btn-close");
  closeButton.addClass("float-end");
  closeButton.empty();
}

function onShow() {
  bootboxVisualFix();
  feather.replace();
}

function isConfirmed(message) {
  return new Promise((resolve, reject) => {
    bootbox.confirm({
      title: "<i data-feather=\"alert-circle\"></i>Внимание",
      message: message,
      centerVertical: true,
      swapButtonOrder: false,
      buttons: {
          cancel: {
              label: 'Отменить',
              className: 'btn-danger'
          },
          confirm: {
              label: 'Да',
              className: 'btn-success'
          },
      },
      onShow: onShow,
      callback: function (result) {
          resolve(result);
      }
    });
  })
}

function onError(context) {
  bootbox.alert({
    title: "<i data-feather=\"x-octagon\"></i>Ошибка",
    message: context.responseText,
    backdrop: true,
    size: 'small',
    className: 'text-danger',
    centerVertical: true,
    onShow: onShow
  });    
}

copypastas.on('click', ".save-button", async function () {
  const result = await isConfirmed("Вы уверены, что хотите сохранить пасту?");
  if (!result) return;

  const copypasta = $(this).closest(".copypasta");
  const copypastaId = copypasta.data("copypasta-id");
  const tags = [];
  copypasta.find(".tags-row .tag").each(function () {
      tags.push($(this).data("tag-id"));
  });
  const data = {
      "Id": copypastaId,
      "Text": copypasta.find(".copypasta__text").val(),
      "tagIds": tags
  };

  const hourglassParts = ["bi-hourglass-top", "bi-hourglass-split", "bi-hourglass-bottom"];
  const size = hourglassParts.length;
  const initialClass = "bi-check-lg";
  let prevClass = initialClass;
  let index = 0;
  const iTag = $(this).find("i");
  const interval = setInterval(() => {
      const currentClass = hourglassParts[index++ % size];
      iTag.toggleClass(`${prevClass} ${currentClass}`);
      prevClass = currentClass;
  }, 0.3 * 1000);

  $.ajax({
      method: "POST",
      url: $(this).data("href"),
      data: data,
      success: function(context) {
          clearInterval(interval);
          iTag.toggleClass(`${prevClass} ${initialClass}`);

          if (copypasta.is(':first-of-type')) {
              location.reload();
          }
      },
      error: onError
  });
});

function clearCopypasta() {
  const copypasta = this;
  const copypastaTextarea = copypasta.find(".copypasta__text");
  copypastaTextarea.prop('value', '');
  copypastaTextarea.trigger("input");
  copypasta.find(".tags-row").empty();
}

copypastas.on('click', ".remove-button", async function () {
  const result = await isConfirmed("Вы уверены, что хотите удалить пасту?");
  if (!result) return;

  const copypasta = $(this).closest(".copypasta");

  if (copypasta.is(':first-child')) {
      clearCopypasta.call(copypasta);
      return;
  }

  const copypastaId = copypasta.data("copypasta-id");
  $.ajax({
      method: "POST",
      url: $(this).data("href"),
      data: { id: copypastaId },
      success: function(context) {
          copypasta.remove();
      },
      error: onError
  });
});

function changeLetterCounter(e) {
  const copypastaTextArea = $<HTMLInputElement>(this);
  const maxLetters = copypastaTextArea.prop("maxlength");
  const letters = (copypastaTextArea.val() as string).length;
  
  copypastaTextArea.siblings(".letter-counter").text(`${letters}/${maxLetters}`);
}

function restrictInput(e) {
  const copypastaTextArea = $(this);
  const text = copypastaTextArea.text();
  const letters = text.length;
  const maxLetters = 500;
  if (letters > maxLetters) {
    e.preventDefault();
  }
}

copypastas.on('input', ".copypasta__text", changeLetterCounter);
copypastas.on('input', ".copypasta__text", fixHeight);

copypastas.on('click', ".tags-container.includable .tag", function() {
  const tag = $(this);
  const tagsRow = tag.closest(".copypasta__footer").find(".tags-row");
  const tagId = tag.data("tag-id");
  
  if (tag.hasClass("included")) {
    const removeSelector = `[data-tag-id="${tagId}"]`;
    tagsRow.find(".tag").remove(removeSelector);
  } else {
    createTag(tagId).prependTo(tagsRow);
  }

  toggleFilterTagState(tag, "included", "excluded");
});

function getAvailableTags() {
  const tagsContainer = $("<div/>", {
    "class": "tags-container includable"
  });

  const usedTagIds = [];
  const footer = $(this).closest(".copypasta__footer");
  const footerTags = footer.find(".tags-row .tag");
  footerTags.each(function () {
    usedTagIds.push($(this).data("tag-id"));
  });
  
  for (const tagInfo of all_tags) {
    const tag = createTag(tagInfo.id);
    tag.appendTo(tagsContainer);
    
    if (usedTagIds.includes(tagInfo.id)) {
      tag.addClass("included");
    }
  }

  return $("<div/>").append(tagsContainer);
}

function addPopoverToButton() {
  const button = this;
  
  tippy(button, {
    trigger: 'click',
    allowHTML: true,
    content: getAvailableTags.call(button).html(),
    interactive: true,
    theme: 'light'
  });

  const footer = $(button).closest(".copypasta__footer");
  const footerTagsRow = footer.find(".tags-row");
  footerTagsRow.on('click', ".tag .tag__remove-button", function () {
    $(this).closest(".tag").remove();
    button._tippy.setContent(getAvailableTags.call(button).html());
  });
}

const all_tags = [];

$(() => {
  $("#filterTagsPopover .tag").each(function()
  {
    const tag = $(this);
    all_tags.push({
      id: tag.data("tag-id"),
      name: tag.text()
    });
  });

  $(".add-tag-button").each(addPopoverToButton);
  
  $(".copypasta:first-of-type .copypasta__text").trigger('focus');

  $(document).on('keydown', function(event) {
    if (event.altKey && event.key == 'F2' && !(event.ctrlKey || event.shiftKey || event.metaKey)) {
      const bootboxElem = bootbox.dialog({
        title: "Выйти?",
        message: "<form action='/Home/Logout' method='GET'> <input type='submit' value='Да'/> </form>",
        size: "small",
        centerVertical: true,
        onEscape: true,
        backdrop: true,
        closeButton: false
      });
    }
  });
});