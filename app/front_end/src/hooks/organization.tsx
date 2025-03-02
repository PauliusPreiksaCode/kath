import { getOrganizations, createOrganization, updateOrganization, deleteOrganization, addUserToOrganization, removeUserFromOrganization, getOrganizationUsers, getNonOrganizationUsers } from '@/services/api';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toastService from '@/services/toast';

export const useGetOrganizations = () => {
    return useQuery({
        queryKey: ['organizations'],
        queryFn: getOrganizations,
        refetchOnWindowFocus: false,
        refetchInterval: false,
    });
}

export const useCreateOrganization = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: createOrganization,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Organization created successfully");
            queryClient.invalidateQueries({ queryKey: ['organizations'] });
        },
    });
}

export const useUpdateOrganization = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: updateOrganization,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Organization updated successfully");
            queryClient.invalidateQueries({ queryKey: ['organizations'] });
        },
    });
}

export const useDeleteOrganization = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: deleteOrganization,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Organization deleted successfully");
            queryClient.invalidateQueries({ queryKey: ['organizations'] });
        },
    });
}

export const useAddUserToOrganization = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: addUserToOrganization,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("User added to organization successfully");
            queryClient.invalidateQueries({ queryKey: ['organizations'] });
        },
    });
}

export const useRemoveUserFromOrganization = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: removeUserFromOrganization,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("User removed from organization successfully");
            queryClient.invalidateQueries({ queryKey: ['organizations'] });
        },
    });
}

export const useGetOrganizationUsers = (id : any) => {
    return useQuery({
        queryKey: ['organizationUsers', id],
        queryFn: () => getOrganizationUsers(id),
        refetchOnWindowFocus: false,
        refetchInterval: false,
    });
}

export const useGetNonOrganizationUsers = (id : any) => {
    return useQuery({
        queryKey: ['nonOrganizationUsers', id],
        queryFn: () => getNonOrganizationUsers(id),
        refetchOnWindowFocus: false,
        refetchInterval: false,
    });
}