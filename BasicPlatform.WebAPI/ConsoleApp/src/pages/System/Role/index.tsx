import { query, detail } from './service';
import { PlusOutlined, FormOutlined } from '@ant-design/icons';
import type { ActionType, ProColumns, ProDescriptionsItemProps } from '@ant-design/pro-components';
import {
  PageContainer,
  ProDescriptions,
  ProTable,
} from '@ant-design/pro-components';
import { FormattedMessage, useIntl, useModel, useLocation, Access } from '@umijs/max';
import { Button, Drawer, message } from 'antd';
import React, { useRef, useState } from 'react';
import IconStatus from '@/components/IconStatus';
import permission from '@/utils/permission';
import { canAccessible, getSorter } from '@/utils/utils';
import CreateOrUpdateForm from './components/CreateOrUpdateForm';


const TableList: React.FC = () => {
  const [createOrUpdateModalOpen, handleCreateOrUpdateModalOpen] = useState<boolean>(false);
  const [showDetail, setShowDetail] = useState<boolean>(false);

  const actionRef = useRef<ActionType>();
  const [currentRow, setCurrentRow] = useState<API.RoleDetailItem>();
  // const [selectedRowsState, setSelectedRows] = useState<API.RoleListItem[]>([]);
  const { getResource } = useModel('resource');
  const location = useLocation();
  const resource = getResource(location.pathname);

  /**
   * @en-US International configuration
   * @zh-CN 国际化配置
   * */
  const intl = useIntl();

  const columns: ProColumns<API.RoleListItem>[] = [
    {
      title: '名称',
      dataIndex: 'name',
      hideInSearch: true,
      width: 150,
    },
    {
      title: '状态',
      dataIndex: 'status',
      width: 90,
      hideInSearch: true,
      sorter: true,
      render(_, entity) {
        return <IconStatus status={entity.status === 1} />;
      },
    },
    {
      title: '备注',
      dataIndex: 'remarks',
      hideInSearch: true,
    },
    {
      title: '更新人',
      dataIndex: 'updatedUserName',
      width: 100,
      hideInSearch: true,
    },
    {
      title: '更新时间',
      dataIndex: 'updatedOn',
      width: 170,
      hideInSearch: true,
      valueType: 'dateTime',
      sorter: true,
    },
    {
      title: '操作',
      dataIndex: 'option',
      valueType: 'option',
      width: 95,
      render(_, entity) {
        return [
          <Button key={'view'}
            shape="circle"
            type={'link'}
            icon={<FormOutlined />}
            onClick={async () => {
              const hide = message.loading('正在查询');
              const res = await detail(entity.id);
              hide();
              if (res.success) {
                setCurrentRow(res.data);
                handleCreateOrUpdateModalOpen(true);
                return;
              }
              message.error(res.message);
            }}>
            编辑
          </Button>,
        ];
      },
    },
    {
      title: '关键字',
      dataIndex: 'keyword',
      hideInTable: true,
      hideInForm: true,
      hideInDescriptions: true,
      hideInSearch: false,
      hideInSetting: true,
      fieldProps: {
        placeholder: '搜索关键字',
      },
    },
  ];

  return (
    <PageContainer header={{
      title: resource?.name,
      children: resource?.description
    }}>
      <ProTable<API.RoleListItem, API.RolePagingParams>
        headerTitle={intl.formatMessage({
          id: 'pages.searchTable.title',
          defaultMessage: 'Enquiry form',
        })}
        actionRef={actionRef}
        rowKey="id"
        search={{
          labelWidth: 120,
        }}
        toolBarRender={() => [
          <Access key={'add'} accessible={canAccessible(permission.role.postAsync, resource)}>
            <Button
              type="primary"
              onClick={() => {
                handleCreateOrUpdateModalOpen(true);
              }}
              icon={<PlusOutlined />}
            >
              <FormattedMessage id="pages.searchTable.new" defaultMessage="New" />
            </Button>
          </Access>,
        ]}
        request={async (params, sorter) => {
          const res = await query({ ...params, ...getSorter(sorter, 'a') });
          return {
            data: res.data?.items || [],
            success: res.success,
            total: res.data?.totalItems || 0,
          }
        }}
        columns={columns}
      // rowSelection={{
      //   onChange: (_, selectedRows) => {
      //     setSelectedRows(selectedRows);
      //   },
      // }}
      />
      <CreateOrUpdateForm
        onCancel={() => {
          handleCreateOrUpdateModalOpen(false);
          if (!showDetail) {
            setCurrentRow(undefined);
          }
        }}
        onSuccess={() => {
          handleCreateOrUpdateModalOpen(false);
          if (!showDetail) {
            setCurrentRow(undefined);
          }
          if (actionRef.current) {
            actionRef.current.reload();
          }
        }}
        open={createOrUpdateModalOpen}
        values={currentRow}
      />
      <Drawer
        width={600}
        open={showDetail}
        onClose={() => {
          setCurrentRow(undefined);
          setShowDetail(false);
        }}
        closable={false}
      >
        {currentRow?.name && (
          <ProDescriptions<API.RoleListItem>
            column={2}
            title={currentRow?.name}
            request={async () => ({
              data: currentRow || {},
            })}
            params={{
              id: currentRow?.name,
            }}
            columns={columns as ProDescriptionsItemProps<API.RoleListItem>[]}
          />
        )}
      </Drawer>
    </PageContainer>
  );
};

export default TableList;
