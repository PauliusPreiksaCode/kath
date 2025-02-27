import instance from "@/lib/axios";
import authService from "./auth";
import { Endpoints } from "@/types";

export async function login(user : any) {
    return await instance
      .post(Endpoints.LOGIN, user)
      .then((res) => {
        const token = res.data;
        authService.setCookies(token);
        return { token };
      })
      .catch(() => {
      });
}

export async function register(user : any) {
    try {
      const response = await instance.post(Endpoints.REGISTER, user);
      return response.data;
    } catch (error) {
    } 
  }

export async function renewToken() {
    return await instance
      .get(Endpoints.RENEWTOKEN)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function logOutUser() {
    return await instance
      .post(Endpoints.LOGOUT)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function getAvailableLicences() {
    return await instance
      .get(Endpoints.LICENCES)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function buyLicence(licence : any) {
    return await instance
      .post(Endpoints.BUY_LICENCE, licence)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function updatePayment(payment : any) {
    return await instance
      .post(Endpoints.UPDATE_PAYMENT, payment)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function getUserLicences() {
    return await instance
      .get(Endpoints.USER_LICENCES)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function getUsers() {
    return await instance
      .get(Endpoints.USERS)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function transferLicence(licence : any) {
    return await instance
      .post(Endpoints.TRANSFER_LICENCE, licence)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function getOrganizations() {
    return await instance
      .get(Endpoints.ORGANIZATIONS)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function createOrganization(organization : any) {
    return await instance
      .post(Endpoints.ORGANIZATION, organization)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function updateOrganization(organization : any) {
    return await instance
      .put(Endpoints.ORGANIZATION, organization)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function deleteOrganization(organization : any) {
    return await instance
      .delete(Endpoints.ORGANIZATION, { data: organization })
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function addUserToOrganization(organizationUser : any) {
    return await instance
      .post(Endpoints.ADD_TO_ORGANIZATION, organizationUser)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function removeUserFromOrganization(organizationUser : any) {
    return await instance
      .post(Endpoints.REMOVE_FROM_ORGANIZATION, organizationUser)
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function getOrganizationUsers(id : any) {
    return await instance
      .get(`${Endpoints.ORGANIZATION_USERS}/${id}`) 
      .then((res) => res.data)
      .catch(() => {
      });
}

export async function getNonOrganizationUsers(id : any) {
    return await instance
      .get(`${Endpoints.NON_ORGANIZATION_USERS}/${id}`)
      .then((res) => res.data)
      .catch(() => {
      });
}
