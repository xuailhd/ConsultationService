using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthCloud.Consultation.Enums;


namespace HealthCloud.Consultation.Dto.Response
{
    /// <summary>
    /// ����ҽ��
    /// </summary>
    public class ResponseDoctorTriageDTO
    {
        /// <summary>
        /// ԤԼID
        /// </summary>
        public string OPDRegisterID { get; set; }

        /// <summary>
        /// ����ҽ��ID
        /// </summary>
        public string TriageDoctorID { get; set; }

        /// <summary>
        /// ����״̬��0�ޣ�1�����2�����У�3�ѷ��
        /// </summary>
        public EnumTriageStatus TriageStatus { get; set; }

        /// <summary>
        /// �Ƿ�Ҫ��������ϵͳ
        /// </summary>
        public bool IsToGuidance { get; set; }

    }
}
