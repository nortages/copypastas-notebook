export const searchParams = {};

export function search(pathName: string) {
  const currentUrl = window.location.href;
  const parsedUrl = new URL(currentUrl);
  parsedUrl.pathname = pathName;
  parsedUrl.searchParams.forEach((val, key) => {
    parsedUrl.searchParams.delete(key);
  });
  
  for (const [key, value] of Object.entries(searchParams)) {
    if (Array.isArray(value)) {
      parsedUrl.searchParams.delete(key);
      for (const arrVal of value) {
        parsedUrl.searchParams.append(key, arrVal.toString());
      }
    } else {
      const valueStr = value.toString();
      if (valueStr === "") {
        parsedUrl.searchParams.delete(key);
      } else {
        parsedUrl.searchParams.set(key, valueStr);
      }
    }
  }
  window.location.href = parsedUrl.toString();
}