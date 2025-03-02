import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { register, getAvailableLicences, buyLicence, updatePayment, getUserLicences, getUsers, transferLicence, removeLicence } from '@/services/api';
import { useNavigate } from 'react-router';
import toastService from '@/services/toast';


export const useRegister = () => {
  const navigate = useNavigate();
  
  return useMutation({
    mutationFn: register,
    onSuccess: (e) => {
      if (e !== undefined)
        toastService.success("User registered successfully");
      navigate('/login');
    },
  });
}

export const useGetLicences = () => {
  return useQuery({
    queryKey: ['licences'],
    queryFn: getAvailableLicences,
    refetchOnWindowFocus: false,
    refetchInterval: false,
  });
};

export const useBuyLicence = () => {  

  return useMutation({
    mutationFn: buyLicence,
    onSuccess: () => {
    }, 
  });
}

export const useUpdatePayment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: updatePayment,
    onSuccess: (e) => {
      if (e !== undefined) 
        toastService.success("Payment updated successfully");
      queryClient.invalidateQueries({ queryKey: ['userLicences'] });
    },
  });
}

export const useGetUserLicences = () => {
  return useQuery({
    queryKey: ['userLicences'],
    queryFn: getUserLicences,
    refetchOnWindowFocus: false,
    refetchInterval: false,
  });
}

export const useGetUsers = () => {
  return useQuery({
    queryKey: ['users'],
    queryFn: getUsers,
    refetchOnWindowFocus: false,
    refetchInterval: false,
  });
}

export const useTransferLicence = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: transferLicence,
    onSuccess: (e) => {
      if (e !== undefined)
        toastService.success("Licence transferred successfully");
      queryClient.invalidateQueries({ queryKey: ['userLicences'] });
    },
  });
}

export const useRemoveLicence = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: removeLicence,
    onSuccess: (e) => {
      if (e !== undefined)
        toastService.success("Licence removed successfully");
      queryClient.invalidateQueries({ queryKey: ['userLicences'] });
    },
  });
}