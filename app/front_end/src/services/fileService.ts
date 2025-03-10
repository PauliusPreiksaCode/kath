
const fileToBlob = (data : any) => {
    const arr = data.split(",");
    const mime = arr[0].match(/:(.*?);/)[1];
    const bstr = atob(arr[1]);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);

    while (n--) {
    u8arr[n] = bstr.charCodeAt(n);
    }

    return new Blob([u8arr], { type: mime });
};

const  getFileNameFromHeaders = (headers: any): string | null => {
    const contentDisposition = headers['content-disposition'];
    if (contentDisposition) {
      const match = contentDisposition.match(/filename=(.+);/);
      return match ? match[1]?.replace(/^[\w\d-]+-/, '') : null;
    }
    return null;
  }
  

export const fileService = {
    fileToBlob,
    getFileNameFromHeaders,
};

