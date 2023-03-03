import { request } from '@umijs/max';

/**
 * 读取分页数据
 * @param url 地址
 * @param data 参数
 * @returns 结果集
 */
export async function paging<T>(url: string, data: any): Promise<ApiPagingResponse<T>> {
  return request(url, {
    method: 'POST',
    data
  });
}

/**
 * POST请求
 * @param url 地址
 * @param params 数据
 * @returns 结果集
 */
export async function post<T>(url: string, data: any): Promise<ApiResponse<T>> {
  return request(url, {
    method: 'POST',
    data
  });
}

/**
 * GET请求
 * @param url 
 * @param params 
 * @returns 
 */
export async function get<T>(url: string, params: any): Promise<ApiResponse<T>> {
  return request(url, {
    method: 'GET',
    params
  });
}

