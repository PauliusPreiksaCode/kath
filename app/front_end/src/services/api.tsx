import instance from "@/lib/axios";
import authService from "./auth";
import { Endpoints } from "@/types";
import toastService from "./toast";
import { AxiosError } from "axios";
import { fileService } from "./fileService";

export async function login(user : any) {
    return await instance
      .post(Endpoints.LOGIN, user)
      .then((res) => {
        const token = res.data;
        authService.setCookies(token);
        return { token };
      })
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function register(user : any) {
    try {
      const response = await instance.post(Endpoints.REGISTER, user);
      return response.data;
    } catch (error) {
      if (error !== undefined) {
        const err = error as AxiosError;
        const errorMessage =
          typeof err.response?.data === "string"
            ? err.response.data
            : JSON.stringify(err.response?.data) || "An unexpected error occurred.";
        toastService.error(errorMessage);
      }
    } 
  }

export async function renewToken() {
    return await instance
      .get(Endpoints.RENEWTOKEN)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function logOutUser() {
    return await instance
      .post(Endpoints.LOGOUT)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getAvailableLicences() {
    return await instance
      .get(Endpoints.LICENCES)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function buyLicence(licence : any) {
    return await instance
      .post(Endpoints.BUY_LICENCE, licence)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function updatePayment(payment : any) {
    return await instance
      .post(Endpoints.UPDATE_PAYMENT, payment)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getUserLicences() {
    return await instance
      .get(Endpoints.USER_LICENCES)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getUsers() {
    return await instance
      .get(Endpoints.USERS)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function transferLicence(licence : any) {
    return await instance
      .post(Endpoints.TRANSFER_LICENCE, licence)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function removeLicence(licence : any) {
    return await instance
      .delete(Endpoints.REMOVE_LICENCE, {data: licence})
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getOrganizations() {
    return await instance
      .get(Endpoints.ORGANIZATIONS)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function createOrganization(organization : any) {
    return await instance
      .post(Endpoints.ORGANIZATION, organization)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function updateOrganization(organization : any) {
    return await instance
      .put(Endpoints.ORGANIZATION, organization)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function deleteOrganization(organization : any) {
    return await instance
      .delete(Endpoints.ORGANIZATION, { data: organization })
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function addUserToOrganization(organizationUser : any) {
    return await instance
      .post(Endpoints.ADD_TO_ORGANIZATION, organizationUser)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function removeUserFromOrganization(organizationUser : any) {
    return await instance
      .post(Endpoints.REMOVE_FROM_ORGANIZATION, organizationUser)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getOrganizationUsers(id : any) {
    return await instance
      .get(`${Endpoints.ORGANIZATION_USERS}/${id}`) 
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getNonOrganizationUsers(id : any) {
    return await instance
      .get(`${Endpoints.NON_ORGANIZATION_USERS}/${id}`)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getOrganizationGroups(id : any) {
    return await instance
      .get(`${Endpoints.GROUPS}/${id}`)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function createGroup(group : any) {
    return await instance
      .post(Endpoints.GROUP, group)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function updateGroup(group : any) {
    return await instance
      .put(Endpoints.GROUP, group)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function deleteGroup(group : any) {
    return await instance
      .delete(Endpoints.GROUP, { data: group })
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getEntries(organizationId : string, groupId : string) {
    return await instance
      .get(`${Endpoints.ENTRIES}/${organizationId}/${groupId}`)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getEntry(organizationId : string, entryId : string) {
    return await instance
      .get(`${Endpoints.ENTRY}/${organizationId}/${entryId}`)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getLinkingEntries(organizationId : string, entryToExclude : string) {
    let path = `${Endpoints.LINKING_ENTRIES}/${organizationId}`;
    if(entryToExclude !== "") {
      path += `?entryToExclude=${entryToExclude}`;
    }

    return await instance
      .get(path)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function getGraphEntries(organizationId : string) {
    return await instance
      .get(`${Endpoints.GRAPH_ENTRIES}/${organizationId}`)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function createEntry(entry : any) {
  const originalContentType = instance.defaults.headers["Content-Type"];

  try {
      const response = await instance.post(Endpoints.ENTRY, entry, { headers: { "Content-Type": "multipart/form-data" } });
      return response.data;
  } catch (error) {
      if (error !== undefined) {
          const err = error as AxiosError;
          const errorMessage =
              typeof err.response?.data === "string"
                  ? err.response.data
                  : JSON.stringify(err.response?.data) || "An unexpected error occurred.";
        toastService.error(errorMessage);
      }
  } 
  finally {
      instance.defaults.headers["Content-Type"] = originalContentType;
  }
}

export async function updateEntry(entry : any) {
  const originalContentType = instance.defaults.headers["Content-Type"];

  try {
      const response = await instance.put(Endpoints.ENTRY, entry, { headers: { "Content-Type": "multipart/form-data" } });
      return response.data;
  } catch (error) {
      if (error !== undefined) {
          const err = error as AxiosError;
          const errorMessage =
              typeof err.response?.data === "string"
                  ? err.response.data
                  : JSON.stringify(err.response?.data) || "An unexpected error occurred.";
        toastService.error(errorMessage);
      }
  } 
  finally {
      instance.defaults.headers["Content-Type"] = originalContentType;
  }
}

export async function deleteEntry(entry : any) {
    return await instance
      .delete(Endpoints.ENTRY, { data: entry })
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function downloadFile(groupId : string, entryId : string) {
  return await instance
    .get(`${Endpoints.DOWNLOAD_FILE}/${groupId}/${entryId}`, { responseType: 'blob' })
    .then((res) => {
      const blob = new Blob([res.data], { type: res.headers['content-type'] });
      const url = window.URL.createObjectURL(blob);
      const filename = fileService.getFileNameFromHeaders(res.headers) || "downloaded-file";
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', filename);
      document.body.appendChild(link);
      link.click();

      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    })
    .catch((e) => {
      if(e !== undefined) {
        toastService.error(e.response.data);
      }
    });
}

export async function deleteFile(groupId : string, entryId : string) {
    return await instance
      .delete(`${Endpoints.DELETE_FILE}/${groupId}/${entryId}`)
      .then((res) => res.data)
      .catch((e) => {
        if(e !== undefined) {
          toastService.error(e.response.data);
        }
      });
}

export async function suggestLinkEntries(request : any) {
  return await instance
    .post(Endpoints.SUGGEST_LINKING_ENTRIES, request)
    .then((res) => res.data)
    .catch((e) => {
      if(e !== undefined) {
        toastService.error(e.response.data);
      }
    });
}