import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toastService from '@/services/toast';
import { createGroup, deleteGroup, getOrganizationGroups, updateGroup } from '@/services/api';

export const useGetGroups = (id: string) => {
    return useQuery({
        queryKey: ['groups', id],
        queryFn: () => getOrganizationGroups(id),
        refetchOnWindowFocus: false,
        refetchInterval: false,
    });
};

export const useCreateGroup = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: createGroup,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Group created successfully");
            queryClient.invalidateQueries({ queryKey: ['groups'] });
        },
    });
};

export const useUpdateGroup = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: updateGroup,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Group updated successfully");
            queryClient.invalidateQueries({ queryKey: ['groups'] });
        },
    });
};

export const useDeleteGroup = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: deleteGroup,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Group deleted successfully");
            queryClient.invalidateQueries({ queryKey: ['groups'] });
        },
    });
};