import { apiCommon } from "../utils/axiosInstance";
export const login = async (username, password) => {
  const response = await apiCommon.post("/Auth/login", {
    username: username.trim(),
    password,
  });

  return response;
};
export const register = async (email, password) => {
  const response = await apiCommon.post("/Auth/Register", {
    email: email.trim(),
    password,
  });

  return response;
};
export const createFolder = async (name, path) => {
  return await apiCommon.post(`/Folders/create`, null, {
    params: {
      name,
      path,
    },
  });
};

export const getFolderContents = async (path = "") => {
  return await apiCommon.get(`/Folders/browse`, {
    params: {
      path: path,
    },
  });
};

export const renameFolder = async (newName, path) => {
  return await apiCommon.put(`/Folders/rename`, null, {
    params: {
      name: newName,
      path: path,
    },
  });
};

export const renameFile = async (newName, path) => {
  return await apiCommon.put(`/Folders/renameFile`, null, {
    params: {
      name: newName,
      path: path,
    },
  });
};

export const deleteFolder = async (path) => {
  return await apiCommon.delete(`/Folders/delete`, {
    params: {
      path: path,
    },
  });
};

export const uploadImage = async (formData) => {
  return await apiCommon.post("/Upload/UploadImage", formData, {});
};
export const getImage = async (path) => {
  return await apiCommon.get("/Upload/getImage", {
    params: {
      imagePath: path,
    },
  });
};

export const deleteImage = async (path) => {
  return await apiCommon.delete("/Upload/DeleteImage", {
    params: {
      imagePath: path,
    },
  });
};
